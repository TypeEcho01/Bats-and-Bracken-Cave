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

        public Farmers Farmers;
        public Tourists Tourists;

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
        }

        public override string ToString() =>
            $"Farmers: {Farmers.Population}\n" +
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