using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static Library.Core.Methods;

namespace BatsAndBrackenCaveSimulation
{
    public class Vendor : Human
    {
        public string Info => ToString();

        public Vendor(string name = "Anonymous Vendor") : base(name)
        {
            Inventory = new Inventory();
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            foreach (var (index, slot) in Enumerate(Inventory, start: 1))
                sb.AppendLine($"{index}) {slot} (Cost: ${slot.Item.Value})");

            return 
                $"Vendor {Name}\n" +
                (Inventory.Length > 0 ? $"Inventory:\n{sb}" : "Empty Inventory");
        }
    }
}