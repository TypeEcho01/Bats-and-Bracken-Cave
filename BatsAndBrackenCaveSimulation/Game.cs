using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatsAndBrackenCaveSimulation
{
    public class Game
    {
        public readonly Player Player;
        public readonly Vendor Vendor;
        public readonly Environment Environment;
        public uint Day = 0;

        public Game()
        {
            DataObject environmentData = Data.Load("Environment");

            Environment = new(
                environmentData.Get<uint>("farmers"),
                environmentData.Get<uint>("tourists"),
                environmentData.Get<uint>("bats"),
                environmentData.Get<uint>("hawks"),
                environmentData.Get<uint>("cornEarworms"),
                environmentData.Get<uint>("cottonBollworms"),
                environmentData.Get<uint>("corn"),
                environmentData.Get<uint>("cotton"),
                environmentData.Get<uint>("dermestidBeetles"),
                environmentData.Get<uint>("guanoBeetles")
            );

            DataObject playerData = Data.Load("Player");

            Player = new(
                playerData.Get<string>("name"),
                null,
                playerData.Get<uint>("currency"),
                playerData.Get<uint>("teamMemberCount"),
                Happiness.Neutral
            );

            DataObject vendorData = Data.Load("Vendor");

            Vendor = new(
                vendorData.Get<string>("name"),
                null
            );
        }

        public void NextDay()
        {
            Day++;
        }
    }
}
