using System;
using System.Collections.Generic;

namespace CBA
{
    public class StatsComponent(Entity owner) : Component(owner)
    {
        public event Action<string>? OnStatChanged;

        private Dictionary<string, (int Base, float Modifier)> _values = new()
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

        public bool HasStat(string name) => _values.ContainsKey(name);
        private void ValidateStat(string name)
        {
            if (!HasStat(name))
                throw new ArgumentException($"Stat '{name}' does not exist.");
        }

        public int Get(string name)
        {
            ValidateStat(name);
            var (Base, Modifier) = _values[name];
            return (int)(Base * Modifier);
        }

        public void IncreaseBase(string name, int delta)
        {
            ValidateStat(name);
            var (Base, Modifier) = _values[name];
            _values[name] = (Base + delta, Modifier);
            OnStatChanged?.Invoke(name);
        }

        public void DecreaseBase(string name, int delta) => IncreaseBase(name, -delta);

        public void IncreaseModifier(string name, float factor)
        {
            ValidateStat(name);
            var (Base, Modifier) = _values[name];
            _values[name] = (Base, Modifier * factor);
            OnStatChanged?.Invoke(name);
        }

        public void DecreaseModifier(string name, float factor) => IncreaseModifier(name, 1f / factor);

        protected override void Subscribe()
        {
            OnStatChanged += name => Printer.PrintStatChanged(this, name);
        }

        public override string ToString()
        {
            string result = "";
            foreach (var kvp in _values)
                result += $"{kvp.Key}: Base {kvp.Value.Base}, Modifier {kvp.Value.Modifier}, Final {Get(kvp.Key)}\n";
            return result;
        }
    }
}
