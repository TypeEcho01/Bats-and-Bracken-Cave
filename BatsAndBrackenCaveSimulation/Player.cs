using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatsAndBrackenCaveSimulation
{
    public class Player : Human
    {
        public string Info => ToString();

        public Team Team;
        public uint Currency;

        public Player(string name = "Anonymous Player", Inventory? inventory = null, uint currency = 0, uint teamMemberCount = 0, Happiness teamHappiness = Happiness.Neutral) : base(name)
        { 
            Currency = currency;
            Team = new Team(teamMemberCount, teamHappiness);

            if (inventory is not null)
                Inventory = inventory;
        }

        public override string ToString() =>
            $"Player {Name}\n" +
            $"{Currency} currency\n" +
            $"{Team.Population} team members\n" + 
            (Inventory.Length > 0 ? $"Inventory:\n{Inventory}" : "Empty Inventory");
    }
}