using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatsAndBrackenCaveSimulation
{
    public class MealwormBag : Item, IUsable
    {
        public MealwormBag() : base("Mealworm Bag", "A 25 pund bag of mealworms to attract and stabilize bat populations.", 1600) { }

        public void Use(Environment environment, out string message)
        {
            message = "The Mealworms were placed into the cave, stablizing the bat population.";
            environment.Bats.Add((uint)(environment.DefaultBatCount * 0.1));
        }
    }
}