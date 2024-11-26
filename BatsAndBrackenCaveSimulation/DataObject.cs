using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Library.Core.Methods;

namespace BatsAndBrackenCaveSimulation
{
    public class DataObject
    {
        public readonly string Name;
        private readonly Dictionary<string, dynamic> _entries = [];

        public DataObject(string name, params (string Name, dynamic Value)[] entries)
        {
            Name = name;
            foreach (var (key, value) in entries)
                _entries.Add(key, value);
        }

        public DataObject(string name, Dictionary<string, dynamic> entries)
        {
            Name = name;
            _entries = entries;
        }

        public T Get<T>(string name)
        {
            if (!_entries.TryGetValue(name, out var value))
                throw new ArgumentException($"No Data entry with name \"{name}\" exists.");

            return CastTo<T>(value);
        }
    }
}
