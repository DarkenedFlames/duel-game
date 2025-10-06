using System;
using System.Collections.Generic;

namespace MyApp
{
    public class Stats
    {
        public event Action<string, int>? OnStatChanged; // name, new effective value

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

        public void IncreaseBase(string name, int delta)
        {
            if (!Values.ContainsKey(name))
                throw new ArgumentException($"Stat '{name}' does not exist.");

            var stat = Values[name];
            Values[name] = (stat.Base + delta, stat.Modifier);
            RaiseChangeEvent(name);
        }

        public void IncreaseModifier(string name, float factor)
        {
            if (!Values.ContainsKey(name))
                throw new ArgumentException($"Stat '{name}' does not exist.");

            var stat = Values[name];
            Values[name] = (stat.Base, stat.Modifier * factor);
            RaiseChangeEvent(name);
        }

        private void RaiseChangeEvent(string name)
        {
            int newValue = Get(name);
            OnStatChanged?.Invoke(name, newValue);
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
