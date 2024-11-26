using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace BatsAndBrackenCaveSimulation
{
    public class Item
    {

        public static readonly Item Empty = new("Item.Empty", string.Empty, 0);

        public static bool IsNullOrEmpty(Item? item) =>
            item is null || item.IsEmpty();

        public readonly string Name;
        public readonly string Description;
        public readonly uint Value;

        public Item(string name, string description, uint value)
        {
            Name = name;
            Description = description;
            Value = value;
        }

        public override string ToString()
        {
            if (IsEmpty())
                return Name;

            return $"{Name}: {Description} (Value: {Value})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Item other)
                return false;

            return
                Name == other.Name &&
                Description == other.Description &&
                Value == other.Value;
        }

        public override int GetHashCode() =>
            HashCode.Combine(Name, Description, Value);

        public static bool operator ==(Item? left, Item? right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Item? left, Item? right) =>
            !(left == right);

        public bool IsEmpty()
            => ReferenceEquals(this, Empty);

        public Item Copy()
        {
            if (IsEmpty())
                throw new InvalidOperationException("Cannot copy Item.Empty");

            return new(Name, Description, Value);
        }
    }
}