using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatsAndBrackenCaveSimulation
{
    public class Team : Group
    {
        public uint DailyCostPerPerson = 200;
        public uint DailyCostTotal;

        public Team(uint initalPopulation, Happiness initalHappieness = Happiness.Neutral) : base(initalPopulation, initalHappieness)
        {
            DailyCostTotal = DailyCostPerPerson * Population;
        }
    }
}