using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatsAndBrackenCaveSimulation
{
    public class Fertilizer : Item, IUsable
    {
        public Fertilizer() : base("Fertilizer", "Increases corn and cotton production by restoring their populations.", 1200) { }

        public void Use(Environment environment, out string message)
        {
            message = "The fertilizer has been applied to the soil, increasing corn and cotton production.";
            environment.Corn.Add((uint)(environment.DefaultCornEarwormCount * 0.1));
            environment.Cotton.Add((uint)(environment.DefaultCottonBollwormCount * 0.1));
        }
    }
}
