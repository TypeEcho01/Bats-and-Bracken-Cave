using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2ConsoleApp
{
    public class Human : Entity
    {
        public Inventory Inventory = new();

        public Human(string name = "Anonymous") : base(name) { }
    }
}