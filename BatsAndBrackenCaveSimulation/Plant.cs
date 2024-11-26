using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatsAndBrackenCaveSimulation
{
    public class Plant : Entity, IOrganism
    {
        public readonly string Species;
        public Happiness Happiness;

        public Plant(string name = "Plant", string species = "Plant", Happiness initialHappiness = Happiness.Neutral) : base(name)
        {
            Species = species;
            Happiness = initialHappiness;
        }
    }
}
