using System;
using System.Collections.Generic;

namespace MyCBA
{
    public class ResourcesComponent : Component
    {
        private readonly Dictionary<string, int> _values = new();

        private readonly Dictionary<string, (float RestorationMultiplier, float ExpenditureMultiplier)> _modifiers = new();

        public event Action<string, int>? OnResourceChanged;
        public event Action<string>? OnResourceDepleted;

        public void InitializeFromStats(StatsComponent stats)
        {
            if (stats.HasStat("MaximumHealth"))
                AddResource("Health", stats.GetStatValue("MaximumHealth"));

            if (stats.HasStat("MaximumStamina"))
                AddResource("Stamina", stats.GetStatValue("MaximumStamina"));
        }

        // ✅ Use named tuple syntax here
        public void AddResource(string name, int maxValue, float restoreMult = 1.0f, float spendMult = 1.0f)
        {
            _values[name] = maxValue;
            _modifiers[name] = (RestorationMultiplier: restoreMult, ExpenditureMultiplier: spendMult);
            OnResourceChanged?.Invoke(name, maxValue);
        }

        public int GetResourceValue(string name)
        {
            if (!_values.TryGetValue(name, out int val))
                return 0;
            return val;
        }

        public void ChangeResourceValue(string name, int delta, StatsComponent? stats = null)
        {
            if (!_values.ContainsKey(name))
                return;

            // ✅ This now works correctly
            var modifiers = _modifiers.ContainsKey(name)
                ? _modifiers[name]
                : (RestorationMultiplier: 1.0f, ExpenditureMultiplier: 1.0f);

            float mult = delta >= 0
                ? modifiers.RestorationMultiplier
                : modifiers.ExpenditureMultiplier;

            int adjusted = (int)(delta * mult);

            int max = stats != null && stats.HasStat($"Maximum{name}")
                ? stats.GetStatValue($"Maximum{name}")
                : int.MaxValue;

            int newValue = Math.Clamp(_values[name] + adjusted, 0, max);
            _values[name] = newValue;

            OnResourceChanged?.Invoke(name, newValue);
            if (newValue <= 0)
                OnResourceDepleted?.Invoke(name);
        }

        public IReadOnlyDictionary<string, int> AllResources => _values;
    }
}
