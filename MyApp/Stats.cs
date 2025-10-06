using System;
using System.Collections.Generic;

namespace MyApp
{
    public class Stats
    {
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

        public int Get(string name)
        {
            if (!Values.ContainsKey(name))
                throw new ArgumentException($"Stat '{name}' does not exist.");

            var stat = Values[name];
            return (int)(stat.Base * stat.Modifier);
        }

        public void IncreaseBase(string name, int newBase)
        {
            if (!Values.ContainsKey(name))
                throw new ArgumentException($"Stat '{name}' does not exist.");

            var stat = Values[name];
            Values[name] = (stat.Base + newBase, stat.Modifier);
        }

        public void IncreaseModifier(string name, float newModifier)
        {
            if (!Values.ContainsKey(name))
                throw new ArgumentException($"Stat '{name}' does not exist.");

            var stat = Values[name];
            Values[name] = (stat.Base, stat.Modifier * newModifier);
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
