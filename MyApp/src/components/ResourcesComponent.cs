namespace CBA
{
    public class ResourcesComponent : Component
    {
        public event Action<string>? OnResourceChanged;
        public event Action<string>? OnResourceDepleted;

        private StatsComponent? _stats;

        private readonly Dictionary<string, (int Value, float RestoreMult, float SpendMult)> _values = new();

        public ResourcesComponent(Entity owner) : base(owner)
        {
            _stats = Owner.GetComponent<StatsComponent>();
            if (_stats != null)
            {
                _values["Health"] = (_stats.Get("MaximumHealth"), 1f, 1f);
                _values["Stamina"] = (_stats.Get("MaximumStamina"), 1f, 1f);
            }
        }

        private void OnStatChange(string statName)
        {
            if (_stats == null) return;

            switch (statName)
            {
                case "MaximumHealth":
                    AdjustResourceForNewMax("Health", _stats.Get(statName));
                    break;
                case "MaximumStamina":
                    AdjustResourceForNewMax("Stamina", _stats.Get(statName));
                    break;
            }
        }

        /// <summary>
        /// Adjusts a resource when its maximum changes: increases the current value by the same delta.
        /// </summary>
        private void AdjustResourceForNewMax(string resourceName, int newMax)
        {
            if (!_values.TryGetValue(resourceName, out var value)) return;

            int oldMax = resourceName switch
            {
                "Health" => _stats!.Get("MaximumHealth") - (newMax - Get("Health")),
                "Stamina" => _stats!.Get("MaximumStamina") - (newMax - Get("Stamina")),
                _ => newMax
            };

            int delta = newMax - oldMax;
            int newValue = Math.Clamp(value.Value + delta, 0, newMax);

            _values[resourceName] = (newValue, value.RestoreMult, value.SpendMult);
            OnResourceChanged?.Invoke(resourceName);
        }


        private void Clamp(string resourceName, int newMax)
        {
            var (Value, RestoreMult, SpendMult) = _values[resourceName];
            int clamped = Math.Clamp(Value, 0, newMax);
            _values[resourceName] = (clamped, RestoreMult, SpendMult);
        }

        public int Get(string name)
        {
            if (!_values.TryGetValue(name, out (int Value, float RestoreMult, float SpendMult) value))
                throw new ArgumentException($"Resource '{name}' does not exist.");
            return value.Value;
        }

        public float GetRestoreMultiplier(string name)
        {
            if (!_values.TryGetValue(name, out (int Value, float RestoreMult, float SpendMult) value))
                throw new ArgumentException($"Resource '{name}' does not exist.");
            return value.RestoreMult;
        }

        public void Change(string name, int delta)
        {
            if (!_values.TryGetValue(name, out (int Value, float RestoreMult, float SpendMult) value))
                throw new ArgumentException($"Resource '{name}' does not exist.");

            var (Value, RestoreMult, SpendMult) = value;
            float multiplier = delta >= 0 ? RestoreMult : SpendMult;
            int adjusted = (int)(delta * multiplier);
            if (_stats != null)
            {
                int max = name switch
                {
                    "Health" => _stats.Get("MaximumHealth"),
                    "Stamina" => _stats.Get("MaximumStamina"),
                    _ => int.MaxValue
                };
                int newVal = Math.Clamp(Value + adjusted, 0, max);

                _values[name] = (newVal, RestoreMult, SpendMult);
            }
            OnResourceChanged?.Invoke(name);
            if (Get(name) <= 0) OnResourceDepleted?.Invoke(name);
        }

        public void ChangeMultiplier(string name, float? restoreFactor = null, float? spendFactor = null)
        {
            if (!_values.TryGetValue(name, out (int Value, float RestoreMult, float SpendMult) value))
                throw new ArgumentException($"Resource '{name}' does not exist.");

            var (Value, RestoreMult, SpendMult) = value;
            float newRestore = RestoreMult * (restoreFactor ?? 1f);
            float newSpend = SpendMult * (spendFactor ?? 1f);

            _values[name] = (Value, newRestore, newSpend);

            OnResourceChanged?.Invoke(name);
            if (Get(name) <= 0) OnResourceDepleted?.Invoke(name);
        }

        public void Refill(string name)
        {
            if (!_values.TryGetValue(name, out (int Value, float RestoreMult, float SpendMult) r))
                throw new ArgumentException($"Resource '{name}' does not exist.");
            if (_stats != null)
            {
                int max = name switch
                {
                    "Health" => _stats.Get("MaximumHealth"),
                    "Stamina" => _stats.Get("MaximumStamina"),
                    _ => int.MaxValue
                };
                _values[name] = (max, r.RestoreMult, r.SpendMult);
                OnResourceChanged?.Invoke(name);
            }
        }

        public override void Subscribe()
        {
            _stats?.OnStatChanged += OnStatChange;
            OnResourceChanged += name => Printer.PrintResourceChanged(this, name);
            OnResourceDepleted += name => Printer.PrintResourceDepleted(this, name);
        }
    }
}
