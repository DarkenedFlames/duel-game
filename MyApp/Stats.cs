using System;
using System.Collections.Generic;

namespace MyApp
{
    public class Stats
    {
        public event Action<string, int>? OnStatChanged;

        public Dictionary<string, (int Base, float Modifier)> Values { get; private set; }

        public Stats()
        {
            Values = new Dictionary<string, (int, float)>
            {
                { "MaximumHealth", (100, 1.0f) },
                { "MaximumStamina", (100, 1.0f) },
                { "Armor", (50, 1.0f) },
                { "Shield", (50, 1.0f) },
                { "Critical", (0, 1.0f) },
                { "Dodge", (0, 1.0f) },
                { "Peer", (0, 1.0f) },
                { "Luck", (0, 1.0f) }
            };
        }

        public bool HasStat(string name) => Values.ContainsKey(name);
        public void ValidateStat(string name)
        {
            if (!HasStat(name))
                throw new ArgumentException($"Stat '{name}' does not exist.");
        }

        public int Get(string name)
        {
            ValidateStat(name);
            var stat = Values[name];
            return (int)(stat.Base * stat.Modifier);
        }

        public void IncreaseBase(string name, int delta)
        {
            ValidateStat(name);
            var stat = Values[name];
            Values[name] = (stat.Base + delta, stat.Modifier);
            OnStatChanged?.Invoke(name, Get(name));
        }

        public void IncreaseModifier(string name, float factor)
        {
            ValidateStat(name);
            var stat = Values[name];
            Values[name] = (stat.Base, stat.Modifier * factor);
            OnStatChanged?.Invoke(name, Get(name));
        }

        public override string ToString()
        {
            string result = "";
            foreach (var kvp in Values)
                result += $"{kvp.Key}: Base {kvp.Value.Base}, Modifier {kvp.Value.Modifier}, Final {Get(kvp.Key)}\n";
            return result;
        }
    }
}
