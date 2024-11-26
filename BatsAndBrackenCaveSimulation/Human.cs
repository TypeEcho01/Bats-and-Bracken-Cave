using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatsAndBrackenCaveSimulation
{
    public class Human : Entity, IOrganism
    {
        public Inventory Inventory = new();

        public Human(string name = "Anonymous") : base(name) { }
    }
}