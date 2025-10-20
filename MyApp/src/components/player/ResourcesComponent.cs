namespace CBA
{
    public class ResourcesComponent : Component
    {
        public event Action<string>? OnResourceChanged;
        public event Action<string>? OnResourceDepleted;

        private readonly StatsComponent _stats;
        private readonly Dictionary<string, (int Value, float RestoreMult, float SpendMult)> _values = [];

        public ResourcesComponent(Entity owner) : base(owner)
        {
            _stats = Owner.GetComponent<StatsComponent>();

            _values["Health"] = (_stats.Get("MaximumHealth"), 1f, 1f);
            _values["Stamina"] = (_stats.Get("MaximumStamina"), 1f, 1f);
        }

        private (int Value, float RestoreMult, float SpendMult) ValidateResource(string name)
        {
            if (!_values.TryGetValue(name, out var value))
                throw new ArgumentException($"Resource '{name}' does not exist.");
            return value;
        }

        private void OnStatChange(string statName)
        {
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

        private void AdjustResourceForNewMax(string resourceName, int newMax)
        {
            (int Value, float RestoreMult, float SpendMult) = ValidateResource(resourceName);

            int oldMax = resourceName switch
            {
                "Health" => _stats!.Get("MaximumHealth") - (newMax - Get("Health")),
                "Stamina" => _stats!.Get("MaximumStamina") - (newMax - Get("Stamina")),
                _ => newMax
            };

            int delta = newMax - oldMax;
            int newValue = Math.Clamp(Value + delta, 0, newMax);

            _values[resourceName] = (newValue, RestoreMult, SpendMult);
            OnResourceChanged?.Invoke(resourceName);
        }

        public int Get(string name)
        {
            (int Value, _, _) = ValidateResource(name);
            return Value;
        }

        public float GetRestoreMultiplier(string name)
        {
            (_, float RestoreMult, _) = ValidateResource(name);
            return RestoreMult;
        }

        public void Change(string name, int delta)
        {
            (int Value, float RestoreMult, float SpendMult) = ValidateResource(name);

            float multiplier = delta >= 0 ? RestoreMult : SpendMult;
            int adjusted = (int)(delta * multiplier);


            int max = name switch
            {
                "Health" => _stats.Get("MaximumHealth"),
                "Stamina" => _stats.Get("MaximumStamina"),
                _ => int.MaxValue
            };

            int newVal = Math.Clamp(Value + adjusted, 0, max);
            _values[name] = (newVal, RestoreMult, SpendMult);
            
            OnResourceChanged?.Invoke(name);
            if (Get(name) <= 0) OnResourceDepleted?.Invoke(name);
        }

        public void ChangeMultiplier(string name, float? restoreFactor = null, float? spendFactor = null)
        {
            (int Value, float RestoreMult, float SpendMult) = ValidateResource(name);

            float newRestore = RestoreMult * (restoreFactor ?? 1f);
            float newSpend = SpendMult * (spendFactor ?? 1f);

            _values[name] = (Value, newRestore, newSpend);

            OnResourceChanged?.Invoke(name);
            if (Get(name) <= 0) OnResourceDepleted?.Invoke(name);
        }

        public void Refill(string name)
        {
            (int Value, float RestoreMult, float SpendMult) = ValidateResource(name);

            int max = name switch
            {
                "Health" => _stats.Get("MaximumHealth"),
                "Stamina" => _stats.Get("MaximumStamina"),
                _ => int.MaxValue
            };

            _values[name] = (max, RestoreMult, SpendMult);
            OnResourceChanged?.Invoke(name);
        }

        public override void Subscribe()
        {
            _stats.OnStatChanged += OnStatChange;
            OnResourceChanged += name => Printer.PrintResourceChanged(this, name);
            OnResourceDepleted += name => Printer.PrintResourceDepleted(this, name);
        }
    }
}
