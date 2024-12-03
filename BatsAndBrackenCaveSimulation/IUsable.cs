using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatsAndBrackenCaveSimulation
{
    internal interface IUsable
    {
        void Use(Environment environment, out string message);
    }
}
