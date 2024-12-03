using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatsAndBrackenCaveSimulation
{
    public class PestSpray : Item, IUsable
    {
        public PestSpray() : base("Pest Spray", "A spray that reduces the number of Corn Earworms and Cotton Bollworms.", 1400) { }

        public void Use(Environment environment, out string message)
        {
            message = "The Pest Spray was used on the farm fields, reducing the pest population.";
            environment.CornEarworms.Subtract((uint)(environment.DefaultCornEarwormCount * 0.1));
            environment.CottonBollworms.Subtract((uint)(environment.DefaultCottonBollwormCount * 0.1));
        }
    }
}