using System;
using System.Collections.Generic;

namespace CBA
{
    public class ResourcesComponent : Component
    {
        public event Action<string>? OnResourceChanged;
        public event Action<string>? OnResourceDepleted;

        private StatsComponent _stats;

        private Dictionary<string, (int Value, float RestoreMult, float SpendMult)> _values = new();

        public ResourcesComponent(Entity owner, StatsComponent stats) : base(owner)
        {
            _stats = stats;

            _values["Health"] = (_stats.Get("MaximumHealth"), 1f, 1f);
            _values["Stamina"] = (_stats.Get("MaximumStamina"), 1f, 1f);

            _stats.OnStatChanged += OnStatChange;
        }

        private void OnStatChange(string statName)
        {
            if (statName == "MaximumHealth") Clamp("Health", _stats.Get(statName));
            if (statName == "MaximumStamina") Clamp("Stamina", _stats.Get(statName));
        }

        private void Clamp(string resourceName, int newMax)
        {
            var r = _values[resourceName];
            int clamped = Math.Clamp(r.Value, 0, newMax);
            _values[resourceName] = (clamped, r.RestoreMult, r.SpendMult);
        }

        public int Get(string name)
        {
            if (!_values.ContainsKey(name))
                throw new ArgumentException($"Resource '{name}' does not exist.");
            return _values[name].Value;
        }

        public float GetRestoreMultiplier(string name)
        {
            if (!_values.ContainsKey(name))
                throw new ArgumentException($"Resource '{name}' does not exist.");
            return _values[name].RestoreMult;
        }

        public void Change(string name, int delta)
        {
            if (!_values.ContainsKey(name))
                throw new ArgumentException($"Resource '{name}' does not exist.");

            var r = _values[name];
            float multiplier = delta >= 0 ? r.RestoreMult : r.SpendMult;
            int adjusted = (int)(delta * multiplier);
            int max = name switch
            {
                "Health" => _stats.Get("MaximumHealth"),
                "Stamina" => _stats.Get("MaximumStamina"),
                _ => int.MaxValue
            };
            int newVal = Math.Clamp(r.Value + adjusted, 0, max);

            _values[name] = (newVal, r.RestoreMult, r.SpendMult);
            OnResourceChanged?.Invoke(name);
            if (Get(name) <= 0) OnResourceDepleted?.Invoke(name);
        }

        public void ChangeMultiplier(string name, float? restoreFactor = null, float? spendFactor = null)
        {
            if (!_values.ContainsKey(name))
                throw new ArgumentException($"Resource '{name}' does not exist.");

            var r = _values[name];
            float newRestore = r.RestoreMult * (restoreFactor ?? 1f);
            float newSpend = r.SpendMult * (spendFactor ?? 1f);

            _values[name] = (r.Value, newRestore, newSpend);

            OnResourceChanged?.Invoke(name);
            if (Get(name) <= 0) OnResourceDepleted?.Invoke(name);
        }

        public void Refill(string name)
        {
            if (!_values.ContainsKey(name))
                throw new ArgumentException($"Resource '{name}' does not exist.");
            var r = _values[name];
            int max = name switch
            {
                "Health" => _stats.Get("MaximumHealth"),
                "Stamina" => _stats.Get("MaximumStamina"),
                _ => int.MaxValue
            };
            _values[name] = (max, r.RestoreMult, r.SpendMult);
            OnResourceChanged?.Invoke(name);
        }

        public override void Subscribe()
        {
            _stats.OnStatChanged += OnStatChange;
            OnResourceChanged += name => Printer.PrintResourceChanged(this, name);
            OnResourceDepleted += name => Printer.PrintResourceDepleted(this, name);
        }

        public override string ToString()
        {
            string result = "";
            foreach (var kvp in _values)
                result += $"{kvp.Key}: {kvp.Value.Value}/{kvp.Key switch { "Health" => _stats.Get("MaximumHealth"), "Stamina" => _stats.Get("MaximumStamina"), _ => kvp.Value.Value }}\n";
            return result;
        }
    }
}
