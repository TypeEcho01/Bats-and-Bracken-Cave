using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2ConsoleApp
{
    public class Player : Human
    {
        public Team Team;
        public Player(string name = "Anonymous Player", uint teamMemberCount = 20, Happiness teamHappiness = Happiness.Neutral) : base(name)
        { 
            Team = new Team(teamMemberCount, teamHappiness);
        }
    }
}