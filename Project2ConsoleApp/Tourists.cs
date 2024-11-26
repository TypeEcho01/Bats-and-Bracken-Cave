using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2ConsoleApp
{
    public class Tourists : Group
    {
        public Tourists(uint initalPopulation, Happiness initalHappieness = Happiness.Neutral) : base(initalPopulation, initalHappieness) { }
    }
}