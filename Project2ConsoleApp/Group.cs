using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2ConsoleApp
{
    public class Group
    {
        public uint Population;
        public Happiness Happiness;

        public Group(uint initalPopulation, Happiness initalHappieness = Happiness.Neutral)
        {
            Population = initalPopulation;
            Happiness = initalHappieness;
        }
    }
}