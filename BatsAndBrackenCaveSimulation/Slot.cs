using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BatsAndBrackenCaveSimulation
{
    public class Slot
    {
        public static readonly Slot Empty = new(Item.Empty, 0);

        public static bool IsNullOrEmpty(Slot? slot) =>
            slot is null || slot.IsEmpty();

        public uint Value => Item.Value * Count;

        public readonly Item Item;
        public uint Count;

        public Slot(Item item, uint count)
        {
            Item = item;
            Count = count;
        }

        public override string ToString()
        {
            if (IsEmpty())
                return "Slot.Empty";

            // Append 's' unless Count is 1 or the name already ends in 's'
            string pluralSuffix = (Count == 1 || Item.Name.EndsWith('s')) ? string.Empty : "s";

            return $"{Count} {Item.Name}{pluralSuffix}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Slot other)
                return false;

            return
                Item == other.Item &&
                Count == other.Count;
        }

        public override int GetHashCode() =>
            HashCode.Combine(Item, Count);

        public static bool operator ==(Slot? left, Slot? right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Slot? left, Slot? right) =>
            !(left == right);

        public bool IsEmpty() =>
            ReferenceEquals(this, Slot.Empty);

        public void Add(uint amount) =>
            Count += amount;

        public void Subtract(uint amount)
        {
            if (Count < amount)
                throw new SlotException($"{nameof(Subtract)} cannot subtract {amount} from Count, as Count is {Count} and cannot be negative.");

            Count -= amount;
        }

        public void Transfer(Slot other, uint amount)
        {
            if (other.Item != Item)
                throw new SlotException($"{nameof(Transfer)} cannot transfer {Item.Name} to other Slot which holds {other.Item.Name}.");

            if (Count < amount)
                throw new SlotException($"{nameof(Transfer)} cannot transfer {amount} from this Slot, as this Slot's Count is {Count} and cannot be negative.");

            Subtract(amount);
            other.Add(amount);
        }
    }
}