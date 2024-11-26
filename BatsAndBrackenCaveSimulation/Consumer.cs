using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatsAndBrackenCaveSimulation
{
    public class Consumer : Animal
    {
        public Consumer(string name = "Consumer", string species = "Animal", Happiness initalHappiness = Happiness.Neutral) : base(name, species, initalHappiness) { }
    }
}