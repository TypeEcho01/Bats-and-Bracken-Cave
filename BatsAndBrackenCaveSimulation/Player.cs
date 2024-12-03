using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static Library.Core.Methods;

namespace BatsAndBrackenCaveSimulation
{
    public class Player : Human
    {
        public string Info => ToString();

        public Team Team;
        public uint Currency;

        public Player(string name = "Anonymous Player", uint currency = 0, uint teamMemberCount = 0, Happiness teamHappiness = Happiness.Neutral) : base(name)
        { 
            Currency = currency;
            Team = new Team(teamMemberCount, teamHappiness);
            Inventory = new Inventory();
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            foreach (var (index, slot) in Enumerate(Inventory, start: 1))
                sb.AppendLine($"{index}) {slot}");

            return
                $"Player\n" +
                $"Currency: ${Currency}\n" +
                (Inventory.Length > 0 ? $"Inventory:\n{sb}" : "Empty Inventory");
        }
    }
}