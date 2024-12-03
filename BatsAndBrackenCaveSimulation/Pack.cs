using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatsAndBrackenCaveSimulation
{
    public class Pack<T> where T : Animal
    {
        public static readonly Pack<Animal> Empty = new(new Animal(), 0);

        public static bool IsNullOrEmpty(Pack<T>? pack) =>
            pack is null || pack.IsEmpty();

        public readonly Animal Animal;
        public uint Count;

        public Pack(T animal, uint count)
        {
            Animal = animal;
            Count = count;
        }

        public override string ToString()
        {
            if (IsEmpty())
                return "Pack.Empty";

            // Append 's' unless Count is 1 or the name already ends in 's'
            string pluralSuffix = (Count == 1 || Animal.Name.EndsWith('s')) ? string.Empty : "s";

            return $"{Count} {Animal.Name}{pluralSuffix}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Pack<T> other)
                return false;

            return
                Animal == other.Animal &&
                Count == other.Count;
        }

        public override int GetHashCode() =>
            HashCode.Combine(Animal, Count);

        public static bool operator ==(Pack<T>? left, Pack<T>? right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Pack<T>? left, Pack<T>? right) =>
            !(left == right);

        public bool IsEmpty() =>
            ReferenceEquals(this, Slot.Empty);

        public void Add(uint amount) =>
            Count += amount;

        public void Subtract(uint amount)
        {
            if (Count < amount)
                Count = 0;
            else
                Count -= amount;
        }

        public void Transfer(Pack<T> other, uint amount)
        {
            if (other.Animal != Animal)
                throw new SlotException($"{nameof(Transfer)} cannot transfer {Animal.Name} to other Pack which holds {other.Animal.Name}.");

            if (Count < amount)
                throw new SlotException($"{nameof(Transfer)} cannot transfer {amount} from this Pack, as this Pack's Count is {Count} and cannot be negative.");

            Subtract(amount);
            other.Add(amount);
        }
    }
}