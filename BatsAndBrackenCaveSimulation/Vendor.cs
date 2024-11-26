using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatsAndBrackenCaveSimulation
{
    public class Vendor : Human
    {
        public string Info => ToString();

        public Vendor(string name = "Anonymous Vendor", Inventory? inventory = null) : base(name)
        {
            if (inventory is not null)
                Inventory = inventory;
        }

        public override string ToString() =>
            $"Vendor {Name}\n" +
            (Inventory.Length > 0 ? $"Inventory:\n{Inventory}" : "Empty Inventory");
    }
}