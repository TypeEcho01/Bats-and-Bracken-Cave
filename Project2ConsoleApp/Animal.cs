using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2ConsoleApp
{
    public class Animal : Entity
    {
        public readonly string Species;
        public Happiness Happiness;

        public Animal(string name = "Animal", string species = "Animal", Happiness initialHappiness = Happiness.Neutral) : base(name) 
        {
            Species = species;
            Happiness = initialHappiness;
        }
    }
}