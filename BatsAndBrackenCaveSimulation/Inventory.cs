﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatsAndBrackenCaveSimulation
{
    public class Inventory
    {
        private readonly List<Slot> _slots;

        public int Length => _slots.Count;

        public Inventory()
        {
            _slots = [];
        }

        public Inventory(params Slot[] slots)
        {
            _slots = [.. slots];
        }

        public void Add(Slot slot) =>
            _slots.Add(slot);

        public Slot Get(Index index) =>
            _slots[index];

        public Slot Get(string itemName)
        {
            foreach (Slot slot in _slots)
            {
                if (slot.Item.Name == itemName)
                    return slot;
            }

            return Slot.Empty;
        }

        public void Remove(Slot slot) =>
            _slots.Remove(slot);

        public void AddAll(params Slot[] slots)
        {
            foreach (Slot slot in slots)
                Add(slot);
        }

        public override string ToString() =>
            string.Join('\n', _slots);
    }
}