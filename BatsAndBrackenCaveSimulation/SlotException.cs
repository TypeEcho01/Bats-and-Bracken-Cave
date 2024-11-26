using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatsAndBrackenCaveSimulation
{
    public class SlotException : Exception
    {
        public SlotException(string? message = null) : base(message) { }
    }
}