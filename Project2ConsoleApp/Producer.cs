using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2ConsoleApp
{
    public class Producer : Animal
    {
        public Producer(string name = "Producer", string species = "Animal", Happiness initalHappiness = Happiness.Neutral) : base(name, species) { }
    }
}