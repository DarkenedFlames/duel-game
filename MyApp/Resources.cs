using System;
using System.Collections.Generic;

namespace MyApp
{
    public class Resources
    {
        private readonly Stats _stats;
        public Dictionary<string, (int Value, float RestoreMult, float SpendMult)> Values { get; private set; }

        public Resources(Stats stats)
        {
            _stats = stats;
            Values = new Dictionary<string, (int, float, float)>
            {
                { "Health", (_stats.Get("MaximumHealth"), 1.0f, 1.0f) },
                { "Stamina", (_stats.Get("MaximumStamina"), 1.0f, 1.0f) }
            };

            // Subscribe to stat changes
            _stats.OnStatChanged += HandleStatChange;
        }

        private void HandleStatChange(string statName, int newValue)
        {
            // Only clamp for stats that affect resources
            if (statName == "MaximumHealth")
                Clamp("Health", newValue);
            else if (statName == "MaximumStamina")
                Clamp("Stamina", newValue);
        }

        private void Clamp(string resourceName, int newMax)
        {
            var resource = Values[resourceName];
            int clamped = Math.Clamp(resource.Value, 0, newMax);
            Values[resourceName] = (clamped, resource.RestoreMult, resource.SpendMult);
        }

        public int Get(string name)
        {
            if (!Values.ContainsKey(name))
                throw new ArgumentException($"Resource '{name}' does not exist.");
            return Values[name].Value;
        }

        public void Change(string name, int delta)
        {
            if (!Values.ContainsKey(name))
                throw new ArgumentException($"Resource '{name}' does not exist.");

            var resource = Values[name];
            float multiplier = delta >= 0 ? resource.RestoreMult : resource.SpendMult;
            int adjusted = (int)(delta * multiplier);

            int max = _stats.Get($"Maximum{name}");
            int newValue = Math.Clamp(resource.Value + adjusted, 0, max);

            Values[name] = (newValue, resource.RestoreMult, resource.SpendMult);
        }

        public void IncreaseMultiplier(string name, float? restoreMult = null, float? spendMult = null)
        {
            if (!Values.ContainsKey(name))
                throw new ArgumentException($"Resource '{name}' does not exist.");

            var resource = Values[name];
            Values[name] = (
                resource.Value,
                resource.RestoreMult * (restoreMult ?? 1),
                resource.SpendMult * (spendMult ?? 1)
            );
        }

        public void Refill(string name)
        {
            if (!Values.ContainsKey(name))
                throw new ArgumentException($"Resource '{name}' does not exist.");

            var resource = Values[name];
            int max = _stats.Get($"Maximum{name}");
            Values[name] = (max, resource.RestoreMult, resource.SpendMult);
        }

        public override string ToString()
        {
            string result = "";
            foreach (var kvp in Values)
                result += $"{kvp.Key}: {kvp.Value.Value}/{_stats.Get($"Maximum{kvp.Key}")}\n";
            return result;
        }
    }
}
