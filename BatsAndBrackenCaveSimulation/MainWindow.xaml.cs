using System;
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

        public MainWindow()
        {
            InitializeComponent();
            SetUp();
        }

        private void SetUp()
        {
            NextDayButton.Visibility = Visibility.Collapsed;
            GameLoop();
            EnvironmentInfoTextBlock.DataContext = Game.Environment;
            VendorInfoTextBlock.DataContext = Game.Vendor;
            PlayerInfoTextBlock.DataContext = Game.Player;
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

                        NextDayButton.Visibility = Visibility.Visible;
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

            // Call the countdown timer again
            Counter();
        }

        private void NextDayButton_Click(object sender, RoutedEventArgs e)
        {
            GameLoop();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}