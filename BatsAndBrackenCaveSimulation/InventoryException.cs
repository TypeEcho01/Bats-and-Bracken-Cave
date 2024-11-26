using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatsAndBrackenCaveSimulation
{
    public class InventoryException : Exception 
    {
        public InventoryException(string? message = null) : base(message) { }
    }
}