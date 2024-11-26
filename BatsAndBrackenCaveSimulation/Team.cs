using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatsAndBrackenCaveSimulation
{
    public class Team : Group
    {
        public Team(uint initalPopulation, Happiness initalHappieness = Happiness.Neutral) : base(initalPopulation, initalHappieness) { }
    }
}