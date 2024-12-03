/*
 * Bats and Bracken Cave Simulation
 * Echo Schwartz
 * 2 December 2024
 * Credits
 * - Random number and shuffling code from PROG 201 class demo
 * - DispatchTimer example by kmatyaszek (https://stackoverflow.com/users/1410998/kmatyaszek)
 */

using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using static BatsAndBrackenCaveSimulation.Utils;
using static Library.WPFApp.Methods;

namespace BatsAndBrackenCaveSimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Game Game = new();
        public DispatcherTimer Timer;
        public TimeSpan TimeSpan;
        public bool IsDaytime = false;
        public bool GameInProgress = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetUp()
        {
            UpdateMainTextBlocks();

            DefaultPrintElement = LogTextBlock;
            Print("Welcome to Bats and Bracken Cave Simulation! Press \"Start Game\" to begin!");
        }

        private void InitializeEnvironmentEvents()
        {
            var environment = Game.Environment;

            // Subscribe handlers to events
            environment.OnLowHawks += HandleLowHawks;
            environment.OnLowCorn += HandleLowCorn;
            environment.OnLowCotton += HandleLowCotton;
            environment.OnLowBats += HandleLowBats;
            environment.OnLowDecomposers += HandleLowDecomposers;
        }

        private void StartDay()
        {
            IsDaytime = true;
            Clear();  // Clears the default print element
            Log("Day", Game.Day, "has begun!");
        }

        private void EndDay()
        {
            IsDaytime = false;
            Log("Day", Game.Day, "has ended!");
            EndOfDayTouristChanges();
            EndOfDayBatChanges();
            AdjustEcosystemCounts();
            AdjustPackPopulationsByDependencies();
            Game.Environment.CheckLowPopulations();
            EndOfDayTeamMemberChanges();
            UpdateMainTextBlocks();

            foreach (Slot slot in Game.Vendor.Inventory)
            {
                slot.Add(1);
            }

            if (GameInProgress)
                NextDayButton.Visibility = Visibility.Visible;
        }

        private void EndOfDayTouristChanges()
        {
            // Retrieve environment and tourist data
            Tourists tourists = Game.Environment.Tourists;
            uint currentPopulation = tourists.Population;
            double defaultPrice = Game.Environment.DefaultTouristCost;
            uint price = Game.Environment.TouristCost;

            // Adjust happiness based on price changes
            AdjustHappinessBasedOnPrice(price, defaultPrice, tourists);

            // Calculate and log population changes based on happiness
            AdjustTouristPopulation(tourists);

            // Calculate and log earnings
            LogEarnings(tourists);

            // Handle tourist dissatisfaction due to bat shortages
            HandleBatDissatisfaction(tourists, currentPopulation);

            // Handle overcrowding
            HandleOvercrowding(tourists, currentPopulation);
        }

        private void AdjustHappinessBasedOnPrice(uint price, double defaultPrice, Tourists tourists)
        {
            double[] upperThresholds = { 1.25, 1.5, 1.75, 2, 2.25, 2.5, 2.75, 3 };
            foreach (var threshold in upperThresholds)
            {
                if (price >= defaultPrice * (threshold + GetRandomVariance(0.1 * threshold)))
                    tourists.Happiness = DecreaseHappiness(tourists.Happiness);
            }

            double[] lowerThresholds = { 0.67, 0.5, 0.33, 0.15 };
            foreach (var threshold in lowerThresholds)
            {
                if (price <= defaultPrice * (threshold + GetRandomVariance(0.1 * threshold)))
                    tourists.Happiness = IncreaseHappiness(tourists.Happiness);
            }
        }

        private void AdjustTouristPopulation(Tourists tourists)
        {
            uint currentPopulation = tourists.Population;
            int newPopulation = (int)(((Game.Environment.DefaultTouristPopulation + currentPopulation) / 2) * GetRandomHappinessMultiplier(tourists.Happiness));

            // All people leave if no one is happy
            if (tourists.Happiness == Happiness.TremendouslyUnhappy)
                newPopulation = 0;

            // Ensure the tourist population does not go below 0
            if (newPopulation < 0)
                newPopulation = 0;

            int populationChange = newPopulation - (int)currentPopulation;

            // Update population
            tourists.Population = (uint)newPopulation;

            // Log population changes
            string happinessString = HappinessToString(tourists.Happiness);
            if (populationChange < 0)
                Log($"Your tourists were {happinessString} and you lost {-populationChange} tourists.");
            else if (populationChange > 0)
                Log($"Your tourists were {happinessString} and you gained {populationChange} tourists.");
            else
                Log($"Your tourists were {happinessString} and your tourist population remained the same.");
        }

        private void LogEarnings(Tourists tourists)
        {
            uint price = Game.Environment.TouristCost;
            uint newPopulation = tourists.Population;
            uint earnings = newPopulation * price;
            Log($"You earned {earnings} dollars from your {newPopulation} tourists.");
            Game.Player.Currency += earnings;
        }

        private void HandleBatDissatisfaction(Tourists tourists, uint currentPopulation)
        {
            uint batCount = Game.Environment.Bats.Count;
            uint defaultBatCount = Game.Environment.DefaultBatCount;

            // If bats are fewer than 75% of default (plus variance), tourists lose happiness
            if (batCount < defaultBatCount * (0.75 + GetRandomVariance(0.05)))
            {
                // Calculate the percentage of bats missing relative to the minimum threshold
                double batDeficitRatio = 1.0 - ((double)batCount / (defaultBatCount * (0.75 + GetRandomVariance(0.05))));
                batDeficitRatio = Math.Clamp(batDeficitRatio, 0, 1); // Ensure it stays between 0 and 1

                // Base tourist loss percentage, increased by how severe the bat deficit is
                double baseLossPercentage = 0.33 + GetRandomVariance(0.09);
                double actualLossPercentage = baseLossPercentage + (0.5 * batDeficitRatio); // Add up to 50% more loss based on deficit

                // Calculate number of tourists leaving
                int unhappyTourists = (int)(currentPopulation * actualLossPercentage);
                if (unhappyTourists > 0)
                {
                    // Ensure population doesn't go negative
                    unhappyTourists = Math.Min(unhappyTourists, (int)tourists.Population);
                    tourists.Population -= (uint)unhappyTourists;

                    Log($"Many tourists were disappointed by the low number of bats, so {unhappyTourists} won't return.");
                }

                // Decrease tourist happiness due to bat shortages
                tourists.Happiness = DecreaseHappiness(tourists.Happiness);
                Log("Tourists were less happy due to the low number of bats.");
            }
        }

        private void HandleOvercrowding(Tourists tourists, uint currentPopulation)
        {
            uint defaultTouristCapacity = Game.Environment.DefaultTouristPopulation * (uint)(1.5 + GetRandomVariance(0.1));
            if (currentPopulation > defaultTouristCapacity)
            {
                // Calculate overcrowding ratio
                double overcrowdingRatio = (double)(currentPopulation - defaultTouristCapacity) / defaultTouristCapacity;
                overcrowdingRatio = Math.Clamp(overcrowdingRatio, 0, 1); // Ensure it stays between 0 and 1

                // Base tourist loss percentage, increased by overcrowding severity
                double baseOvercrowdingLoss = 0.20 + GetRandomVariance(0.05); // Base 20% (plus variance)
                double overcrowdingLossPercentage = baseOvercrowdingLoss + (0.4 * overcrowdingRatio); // Add up to 40% more loss

                // Calculate number of tourists leaving due to overcrowding
                int overwhelmedTourists = (int)(currentPopulation * overcrowdingLossPercentage);
                if (overwhelmedTourists > 0)
                {
                    // Ensure population doesn't go negative
                    overwhelmedTourists = Math.Min(overwhelmedTourists, (int)tourists.Population);
                    tourists.Population -= (uint)overwhelmedTourists;

                    Log($"The cave became overcrowded with tourists, causing {overwhelmedTourists} to leave and not return.");
                }

                // Decrease tourist happiness due to overcrowding
                tourists.Happiness = DecreaseHappiness(tourists.Happiness);
                Log("Tourists were less happy due to the overcrowding.");
            }
        }

        private void EndOfDayBatChanges()
        {
            // Retrieve environment variables
            Pack<Consumer> bats = Game.Environment.Bats;
            uint defaultBatCount = Game.Environment.DefaultBatCount;
            uint touristPopulation = Game.Environment.Tourists.Population;
            uint defaultTouristPopulation = Game.Environment.DefaultTouristPopulation;

            // Initialize change in bat population
            int populationChange = 0;

            // Handle tourist impact on bat population (reducing population)
            double[] upperThresholds = { 1.5, 2.0, 2.5, 3.0, 3.5, 4, 4.5, 5 };
            foreach (var threshold in upperThresholds)
            {
                if (touristPopulation >= defaultTouristPopulation * (threshold + GetRandomVariance(0.1 * threshold)))
                    populationChange -= (int)(defaultBatCount * (0.05 + GetRandomVariance(0.025)));
            }

            // Handle tourist impact on bat population (increasing population)
            double[] lowerThresholds = { 0.67, 0.5, 0.33 };
            foreach (var threshold in lowerThresholds)
            {
                if (touristPopulation <= defaultTouristPopulation * (threshold + GetRandomVariance(0.1 * threshold)))
                    populationChange += (int)(defaultBatCount * (0.05 + GetRandomVariance(0.025)));
            }

            // Calculate the new bat population
            int updatedBatCount = (int)bats.Count + populationChange;

            // Ensure the bat population does not go below 0
            if (updatedBatCount < 0)
            {
                populationChange = -(int)bats.Count; // All bats leave the cave
                updatedBatCount = 0;
            }

            // Log changes
            if (populationChange > 0)
                Log($"The bats were comfortable with the number of tourists.");
            else if (populationChange < 0)
                Log($"The bats were scared by the number of tourists.");
            else
                Log("The bats were contempt with the number of tourists.");

            if (populationChange > 0)
            {
                bats.Add((uint)populationChange);
                Log($"Bat population increased by {populationChange}.");
            }
            else if (populationChange < 0)
            {
                bats.Subtract((uint)Math.Abs(populationChange));
                Log($"Bat population decreased by {Math.Abs(populationChange)}.");
            }
        }

        private void EndOfDayTeamMemberChanges()
        {
            uint costPerPerson = Game.Player.Team.DailyCostPerPerson;
            uint totalCost = Game.Player.Team.DailyCostTotal;

            if (totalCost > Game.Player.Currency)
            {
                GameInProgress = false;
                Log("You do not have enough money to pay your team. Game over.");
                Game.Player.Currency = 0;
                return;
            }

            Game.Player.Currency -= totalCost;

            Log($"You have payed your team members ${costPerPerson} per person. (Total: ${totalCost})");
        }

        private void AdjustEcosystemCounts()
        {
            AdjustConsumerPack(Game.Environment.Hawks, Game.Environment.DefaultHawkCount, "Hawks");
            AdjustConsumerPack(Game.Environment.CornEarworms, Game.Environment.DefaultCornEarwormCount, "Corn earworms");
            AdjustConsumerPack(Game.Environment.CottonBollworms, Game.Environment.DefaultCottonBollwormCount, "Cotton bollworms");

            AdjustProducerPack(Game.Environment.Corn, Game.Environment.DefaultCornCount, "Corn");
            AdjustProducerPack(Game.Environment.Cotton, Game.Environment.DefaultCottonCount, "Cotton");

            AdjustDecomposerPack(Game.Environment.DermestidBeetles, Game.Environment.DefaultDermestidBeetleCount, "Dermestid beetles");
            AdjustDecomposerPack(Game.Environment.GuanoBeetles, Game.Environment.DefaultGuanoBeetleCount, "Guano beetles");
        }

        private void AdjustConsumerPack(Pack<Consumer> pack, uint defaultCount, string name)
        {
            int change = GetRandomChangeBasedOnDefault(defaultCount);
            try
            {
                if (change > 0)
                {
                    pack.Add((uint)change);
                    Log($"{name} population increased by {change}.");
                }
                else if (change < 0)
                {
                    pack.Subtract((uint)Math.Abs(change));
                    Log($"{name} population decreased by {Math.Abs(change)}.");
                }
            }
            catch (SlotException e)
            {
                Log($"{name} population could not decrease further: {e.Message}");
            }
        }

        private void AdjustProducerPack(Pack<Producer> pack, uint defaultCount, string name)
        {
            int change = GetRandomChangeBasedOnDefault(defaultCount);
            try
            {
                if (change > 0)
                {
                    pack.Add((uint)change);
                    Log($"{name} growth increased by {change}.");
                }
                else if (change < 0)
                {
                    pack.Subtract((uint)Math.Abs(change));
                    Log($"{name} growth decreased by {Math.Abs(change)}.");
                }
            }
            catch (SlotException e)
            {
                Log($"{name} growth could not decrease further: {e.Message}");
            }
        }

        private void AdjustDecomposerPack(Pack<Decomposer> pack, uint defaultCount, string name)
        {
            int change = GetRandomChangeBasedOnDefault(defaultCount);
            try
            {
                if (change > 0)
                {
                    pack.Add((uint)change);
                    Log($"{name} activity increased by {change}.");
                }
                else if (change < 0)
                {
                    pack.Subtract((uint)Math.Abs(change));
                    Log($"{name} activity decreased by {Math.Abs(change)}.");
                }
            }
            catch (SlotException e)
            {
                Log($"{name} activity could not decrease further: {e.Message}");
            }
        }

        private int GetRandomChangeBasedOnDefault(uint defaultCount)
        {
            // Calculate a random change as a percentage of the default count
            Random random = new Random();
            double maxPercentageChange = 0.25; // Maximum change + or - 25% of the default
            double randomPercentage = random.NextDouble() * maxPercentageChange; // Random percentage within the range

            // Determine whether the change is positive or negative
            bool isIncrease = random.Next(2) == 0;
            int change = (int)(defaultCount * randomPercentage);
            return isIncrease ? change : -change;
        }

        // Adds interactions between packs based on logical dependencies
        private void AdjustPackPopulationsByDependencies()
        {
            AdjustCornAndEarworms();
            AdjustCottonAndBollworms();
            AdjustHawksAndPests();
            AdjustBeetlesAndGuano();
        }

        // Corn Earworms reduce Corn population
        private void AdjustCornAndEarworms()
        {
            uint cornCount = Game.Environment.Corn.Count;
            uint earwormCount = Game.Environment.CornEarworms.Count;

            if (earwormCount > 0)
            {
                uint cornLost = Math.Min(cornCount, (uint)(earwormCount * 0.1)); // Each earworm destroys up to 0.1 units of corn
                Game.Environment.Corn.Subtract(cornLost);

                Log($"{cornLost} Corn units were consumed by {earwormCount} Corn Earworms.");

                // Earworms population grows based on available food
                uint earwormGrowth = (uint)(cornCount * 0.05); // Each unit of corn can support 0.05 more earworms
                Game.Environment.CornEarworms.Add(earwormGrowth);

                Log($"Corn availability allowed Corn Earworm population to grow by {earwormGrowth}.");
            }
            else
            {
                Log("No Corn Earworms were present to impact Corn production.");
            }
        }

        // Cotton Bollworms reduce Cotton population
        private void AdjustCottonAndBollworms()
        {
            uint cottonCount = Game.Environment.Cotton.Count;
            uint bollwormCount = Game.Environment.CottonBollworms.Count;

            if (bollwormCount > 0)
            {
                uint cottonLost = Math.Min(cottonCount, (uint)(bollwormCount * 0.1)); // Each bollworm destroys up to 0.1 units of cotton
                Game.Environment.Cotton.Subtract(cottonLost);

                Log($"{cottonLost} Cotton units were consumed by {bollwormCount} Cotton Bollworms.");

                // Bollworm population grows based on available food
                uint bollwormGrowth = (uint)(cottonCount * 0.05); // Each unit of cotton can support 0.05 more bollworms
                Game.Environment.CottonBollworms.Add(bollwormGrowth);

                Log($"Cotton availability allowed Cotton Bollworm population to grow by {bollwormGrowth}.");
            }
            else
            {
                Log("No Cotton Bollworms were present to impact Cotton production.");
            }
        }

        // Hawks reduce Corn Earworms and Cotton Bollworms
        private void AdjustHawksAndPests()
        {
            uint hawkCount = Game.Environment.Hawks.Count;
            uint earwormCount = Game.Environment.CornEarworms.Count;
            uint bollwormCount = Game.Environment.CottonBollworms.Count;

            if (hawkCount > 0 && (earwormCount > 0 || bollwormCount > 0))
            {
                uint earwormsEaten = Math.Min(earwormCount, (uint)(hawkCount * 2)); // Each hawk eats up to 2 earworms
                uint bollwormsEaten = Math.Min(bollwormCount, (uint)(hawkCount * 2)); // Each hawk eats up to 2 bollworms

                Game.Environment.CornEarworms.Subtract(earwormsEaten);
                Game.Environment.CottonBollworms.Subtract(bollwormsEaten);

                Log($"Hawks hunted pests, eating {earwormsEaten} Corn Earworms and {bollwormsEaten} Cotton Bollworms.");

                // Hawks population grows or declines based on food availability
                if (earwormsEaten + bollwormsEaten < hawkCount)
                {
                    uint starvedHawks = hawkCount - (earwormsEaten + bollwormsEaten);
                    Game.Environment.Hawks.Subtract(starvedHawks);
                    Log($"Insufficient food caused {starvedHawks} Hawks to leave or perish.");
                }
            }
            else
            {
                if (hawkCount > 0)
                {
                    uint starvedHawks = hawkCount;
                    Game.Environment.Hawks.Subtract(starvedHawks);
                    Log($"{starvedHawks} Hawks starved due to a lack of pests.");
                }

                if (earwormCount > 0 || bollwormCount > 0)
                {
                    uint pestGrowth = (uint)((earwormCount + bollwormCount) * 0.2);
                    Game.Environment.CornEarworms.Add(pestGrowth / 2);
                    Game.Environment.CottonBollworms.Add(pestGrowth / 2);
                    Log($"With no Hawks to hunt them, pests increased by {pestGrowth}.");
                }
            }
        }

        // Decomposers increase if there's enough guano
        private void AdjustBeetlesAndGuano()
        {
            uint dermestidCount = Game.Environment.DermestidBeetles.Count;
            uint guanoCount = Game.Environment.GuanoBeetles.Count;
            uint batCount = Game.Environment.Bats.Count;

            if (batCount > 0)
            {
                uint guanoProduced = (uint)(batCount * 0.01); // Bats produce guano

                Game.Environment.DermestidBeetles.Add(guanoProduced / 2);
                Game.Environment.GuanoBeetles.Add(guanoProduced / 2);

                Log($"Bats increased decomposer populations: Dermestid Beetles grew by {guanoProduced / 2}, and Guano Beetles grew by {guanoProduced / 2}.");
            }
            else
            {
                Log("No bats were present to produce guano, so decomposer populations remained stable.");
            }

            // Decomposers starve if guano is insufficient
            if (dermestidCount > 0 || guanoCount > 0)
            {
                uint decomposersStarved = (uint)((dermestidCount + guanoCount) * 0.1);
                Game.Environment.DermestidBeetles.Subtract(decomposersStarved / 2);
                Game.Environment.GuanoBeetles.Subtract(decomposersStarved / 2);

                Log($"{decomposersStarved} decomposers starved due to insufficient guano.");
            }
        }

        private void HandleLowHawks(uint hawkCount)
        {
            uint pestGrowth = (uint)((Game.Environment.CornEarworms.Count + Game.Environment.CottonBollworms.Count) * 0.5);
            Game.Environment.CornEarworms.Add(pestGrowth / 2);
            Game.Environment.CottonBollworms.Add(pestGrowth / 2);

            Log($"Low Hawk population ({hawkCount}) caused pests to increase significantly by {pestGrowth}.");
        }

        private void HandleLowCorn(uint cornCount)
        {
            uint earwormLoss = Math.Min(Game.Environment.CornEarworms.Count, (uint)(Game.Environment.CornEarworms.Count * 0.3));
            Game.Environment.CornEarworms.Subtract(earwormLoss);

            Log($"Low Corn population ({cornCount}) led to a decline of {earwormLoss} Corn Earworms due to lack of food.");
        }

        private void HandleLowCotton(uint cottonCount)
        {
            uint bollwormLoss = Math.Min(Game.Environment.CottonBollworms.Count, (uint)(Game.Environment.CottonBollworms.Count * 0.3));
            Game.Environment.CottonBollworms.Subtract(bollwormLoss);

            Log($"Low Cotton population ({cottonCount}) led to a decline of {bollwormLoss} Cotton Bollworms due to lack of food.");
        }

        private void HandleLowBats(uint batCount)
        {
            uint guanoLoss = (uint)((Game.Environment.DermestidBeetles.Count + Game.Environment.GuanoBeetles.Count) * 0.5);
            Game.Environment.DermestidBeetles.Subtract(guanoLoss / 2);
            Game.Environment.GuanoBeetles.Subtract(guanoLoss / 2);

            Log($"Low Bat population ({batCount}) caused decomposer populations to shrink by {guanoLoss} due to lack of guano.");
        }
        
        private void HandleLowDecomposers(uint dermestidCount, uint guanoCount)
        {
            uint soilNutrientLoss = (uint)((dermestidCount + guanoCount) * 0.2);
            uint cropLoss = Math.Min(Game.Environment.Corn.Count + Game.Environment.Cotton.Count, soilNutrientLoss);

            Game.Environment.Corn.Subtract(cropLoss / 2);
            Game.Environment.Cotton.Subtract(cropLoss / 2);

            Log($"Low decomposer populations ({dermestidCount} Dermestid Beetles, {guanoCount} Guano Beetles) caused soil nutrients to decline, reducing crops by {cropLoss}.");
        }

        private void Counter()
        {
            NextDayButton.Visibility = Visibility.Collapsed;

            // DispatchTimer example by kmatyaszek (https://stackoverflow.com/users/1410998/kmatyaszek)
            TimeSpan = TimeSpan.FromSeconds(60);

            Timer = new DispatcherTimer(
                new TimeSpan(0, 0, 1),
                DispatcherPriority.Normal,
                delegate
                {
                    TimeTextBlock.Text = TimeSpan.ToString("c");

                    if (TimeSpan == TimeSpan.Zero)
                    {
                        Timer.Stop();

                        EndDay();
                    }

                    TimeSpan = TimeSpan.Add(TimeSpan.FromSeconds(-1));
                },
                Application.Current.Dispatcher);

            Timer.Start();
        }

        private void GameLoop()
        {
            // Move the game forward 
            Game.NextDay();

            // Update UI
            DayTextBlock.Text = $"Day: {Game.Day}";
            StartDay();

            // Call the countdown timer again
            Counter();
        }

        private void UpdateMainTextBlocks()
        {
            EnvironmentInfoTextBlock.Text = Game.Environment.ToString();
            VendorInfoTextBlock.Text = Game.Vendor.ToString();
            PlayerInfoTextBlock.Text = Game.Player.ToString();
            TouristTextBlock.Text = $"${Game.Environment.TouristCost}";
        }

        private void Log(params object?[]? args)
        {
            string logText = LogTextBlock.Text;

            Clear(LogTextBlock);

            // puts the log in reverse order
            Print(args);
            Print(logText);
        }

        private void NextDayButton_Click(object sender, RoutedEventArgs e)
        {
            GameLoop();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            SetUp();
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDaytime)
            {
                Log("You cannot buy anything while the day is over.");
                return;
            }

            string itemNumber = Input(ItemNumberTextBox);
            string amountToBuy = Input(AmountToBuyTextBox);

            if (itemNumber == string.Empty)
            {
                Log("An item number must be entered before buying from the vendor.");
                return;
            }

            if (!int.TryParse(itemNumber, out int index))
            {
                Log($"\"{itemNumber}\" is not a valid item number.");
                return;
            }

            index--;

            Slot slot = Game.Vendor.Inventory.Get(index);

            if (slot.IsEmpty())
            {
                Log("Item number", index + 1, "does not exist in the vendor's inventory.");
                return;
            }

            if (amountToBuy == string.Empty)
            {
                Log("An amount to buy must be entered before buying from the vendor.");
                return;
            }

            if (!int.TryParse(amountToBuy, out int amount))
            {
                Log($"\"{amountToBuy}\" is not a valid amount to buy.");
                return;
            }

            if (slot.Count < amount)
            {
                Log("The vendor does not have enough", slot.Item.Name, "for you to buy", amount, "from them.");
                return;
            }

            Item item = slot.Item;
            uint totalCost = (uint)(item.Value * amount);

            if (totalCost > Game.Player.Currency)
            {
                Log("You cannot afford to buy", item.Name, 'x', amount, "from the vendor.");
                return;
            }

            uint uintAmount = (uint)amount;

            if (Game.Player.Inventory.Get(item.Name).IsEmpty())
                Game.Player.Inventory.Add(new Slot(item, uintAmount));
            else
                Game.Player.Inventory.Get(item.Name).Add(uintAmount);

            Game.Vendor.Inventory.Get(index).Subtract(uintAmount);

            Game.Player.Currency -= totalCost;

            Log("You successfully bought", item.Name, 'x', amount, "from the vendor.");

            UpdateMainTextBlocks();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Clear(ItemNumberTextBox);
            Clear(AmountToBuyTextBox);
        }

        private void SetPriceButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDaytime)
            {
                Log("You cannot set the tourist price while the day is over.");
                return;
            }

            string input = Input(TouristPriceTextBox);

            if (input == string.Empty)
            {
                Log("You must enter a price before setting a tourist price.");
                return;
            }

            if (!int.TryParse(input, out int price) || price < 0)
            {
                Log($"\"{input}\" is not a valid tourist price.");
                return;
            }

            Game.Environment.TouristCost = (uint)price;
            Clear(TouristPriceTextBox);

            Log($"Tourist price is now set to ${price}!");

            UpdateMainTextBlocks();
        }

        private void UseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDaytime)
            {
                Log("You cannot use anything while the day is over.");
                return;
            }

            string itemNumber = Input(UseItemNumberTextBox);

            if (itemNumber == string.Empty)
            {
                Log("An item number must be entered before buying from the vendor.");
                return;
            }

            if (!int.TryParse(itemNumber, out int index))
            {
                Log($"\"{itemNumber}\" is not a valid item number.");
                return;
            }

            index--;

            Slot slot = Game.Player.Inventory.Get(index);

            if (slot.IsEmpty())
            {
                Log("Item number", index + 1, "does not exist in your inventory.");
                return;
            }

            if (slot.Count < 1)
            {
                Log("You do not have enough", slot.Item.Name, "for you to use it.");
                return;
            }

            Item item = slot.Item;

            if (item is not IUsable useableItem)
            {
                Log(slot.Item.Name, "cannot be used.");
                return;
            }

            Game.Player.Inventory.Get(index).Subtract(1);

            useableItem.Use(Game.Environment, out string message);

            Log(message);

            UpdateMainTextBlocks();
        }
    }
}