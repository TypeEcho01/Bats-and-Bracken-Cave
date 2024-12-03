using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatsAndBrackenCaveSimulation
{
    public class Environment
    {
        public string Info => ToString();

        public Pack<Consumer> Bats;
        public Pack<Consumer> Hawks;
        public Pack<Consumer> CornEarworms;
        public Pack<Consumer> CottonBollworms;

        public Pack<Producer> Corn;
        public Pack<Producer> Cotton;
        
        public Pack<Decomposer> DermestidBeetles;
        public Pack<Decomposer> GuanoBeetles;

        public Farmers Farmers; // unused
        public Tourists Tourists;

        public uint DefaultTouristCost = 50;
        public uint TouristCost;
        public uint DefaultTouristPopulation;

        public uint DefaultBatCount;
        public uint DefaultHawkCount;
        public uint DefaultCornEarwormCount;
        public uint DefaultCottonBollwormCount;
        public uint DefaultCottonCount;
        public uint DefaultCornCount;
        public uint DefaultDermestidBeetleCount;
        public uint DefaultGuanoBeetleCount;

        public event Action<uint> OnLowHawks;
        public event Action<uint> OnLowCorn;
        public event Action<uint> OnLowCotton;
        public event Action<uint> OnLowBats;
        public event Action<uint, uint> OnLowDecomposers;

        public Environment(uint farmers, uint tourists, uint bats, uint hawks, uint cornEarworms, uint cottonBollworm, uint corn, uint cotton, uint dermestidBeetles, uint guanoBeetles)
        {
            Bats = new(new Consumer("Bat"), bats);
            Hawks = new(new Consumer("Hawk"), hawks);
            CornEarworms = new(new Consumer("Corn Earworm"), cornEarworms);
            CottonBollworms = new(new Consumer("Cotton Bollworm"), cottonBollworm);

            Cotton = new(new Producer("Cotton"), cotton);
            Corn = new(new Producer("Corn"), corn);

            DermestidBeetles = new(new Decomposer("Dermestid Beetle"), dermestidBeetles);
            GuanoBeetles = new(new Decomposer("Guano Beetle"), guanoBeetles);

            Farmers = new(farmers);
            Tourists = new(tourists);

            TouristCost = DefaultTouristCost;
            DefaultTouristPopulation = Tourists.Population;

            DefaultBatCount = Bats.Count;
            DefaultHawkCount = Hawks.Count;
            DefaultCornEarwormCount = CornEarworms.Count;
            DefaultCottonBollwormCount = CottonBollworms.Count;
            DefaultCottonCount = Cotton.Count;
            DefaultCornCount = Corn.Count;
            DefaultGuanoBeetleCount = GuanoBeetles.Count;
        }

        public void CheckLowPopulations()
        {
            if (Hawks.Count < DefaultHawkCount * 0.2)
                OnLowHawks?.Invoke(Hawks.Count);

            if (Corn.Count < DefaultCornCount * 0.2)
                OnLowCorn?.Invoke(Corn.Count);

            if (Cotton.Count < DefaultCottonCount * 0.2)
                OnLowCotton?.Invoke(Cotton.Count);

            if (Bats.Count < DefaultBatCount * 0.2)
                OnLowBats?.Invoke(Bats.Count);

            if (DermestidBeetles.Count < DefaultDermestidBeetleCount * 0.2 ||
                GuanoBeetles.Count < DefaultGuanoBeetleCount * 0.2)
                OnLowDecomposers?.Invoke(DermestidBeetles.Count, GuanoBeetles.Count);
        }

        public override string ToString() =>
            $"Tourists: {Tourists.Population}\n" +
            $"Bats: {Bats.Count}\n" +
            $"Hawks: {Hawks.Count}\n" +
            $"Corn Earworms: {CornEarworms.Count}\n" +
            $"Cottom Bollworm: {CottonBollworms.Count}\n" +
            $"Corn: {Corn.Count}\n" +
            $"Cotton: {Cotton.Count}\n" +
            $"Dermestid Beetles: {DermestidBeetles.Count}\n" +
            $"Guano Beetles: {GuanoBeetles.Count}";
    }
}