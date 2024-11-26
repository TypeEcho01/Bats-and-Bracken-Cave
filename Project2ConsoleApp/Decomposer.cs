using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2ConsoleApp
{
    public class Decomposer : Animal
    {
        public Decomposer(string name = "Decomposer", string species = "Animal", Happiness initalHappiness = Happiness.Neutral) : base(name, species) { }
    }
}