using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2ConsoleApp
{
    public class Environment
    {
        public int Temperature;
        public uint BatCount;
        public byte Month;
        public byte Day;

        public List<Producer> Producers;
        public List<Consumer> Consumers;
        public List<Decomposer> Decomposers;

        public Player Player;
        public Vendor Vendor;

        public Farmers Farmers;
        public Tourists Tourists;

        public Environment(int initalTemperature, uint batCount, byte startingMonth = 1, byte startingDay = 1)
        { 
            Temperature = initalTemperature;
            BatCount = batCount;
            Month = startingMonth;
            Day = startingDay;

            Producers = [];
            Consumers = [];
            Decomposers = [];

            Player = new();
            Vendor = new();

            Farmers = new(10);
            Tourists = new(50);
        }
    }
}