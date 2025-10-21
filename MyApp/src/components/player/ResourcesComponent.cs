using System.Runtime.CompilerServices;

namespace CBA
{
    public class ResourcesComponent(Entity owner) : Component(owner)
    {
        public event Action<string>? OnResourceChanged;
        public event Action<string>? OnResourceDepleted;

        private readonly Dictionary<string, (int Value, float RestoreMult, float SpendMult)> _values = [];

        private StatsComponent Stats() => Owner.GetComponent<StatsComponent>();

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
                    AdjustResourceForNewMax("Health", Stats().Get(statName));
                    break;
                case "MaximumStamina":
                    AdjustResourceForNewMax("Stamina", Stats().Get(statName));
                    break;
            }
        }

        private void AdjustResourceForNewMax(string resourceName, int newMax)
        {
            (int Value, float RestoreMult, float SpendMult) = ValidateResource(resourceName);

            int oldMax = resourceName switch
            {
                "Health" => Stats().Get("MaximumHealth") - (newMax - Get("Health")),
                "Stamina" => Stats().Get("MaximumStamina") - (newMax - Get("Stamina")),
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
                "Health" => Stats().Get("MaximumHealth"),
                "Stamina" => Stats().Get("MaximumStamina"),
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
                "Health" => Stats().Get("MaximumHealth"),
                "Stamina" => Stats().Get("MaximumStamina"),
                _ => int.MaxValue
            };

            _values[name] = (max, RestoreMult, SpendMult);
            OnResourceChanged?.Invoke(name);
        }

        protected override void RegisterSubscriptions()
        {
            // Not actual subscription logic... might mean stats and resources need to be merged
            _values["Health"] = (Stats().Get("MaximumHealth"), 1f, 1f);
            _values["Stamina"] = (Stats().Get("MaximumStamina"), 1f, 1f);

            // Subscribe Logic
            RegisterSubscription<Action<string>>(
                h => Stats().OnStatChanged += h,
                h => Stats().OnStatChanged -= h,
                OnStatChange
            );
            RegisterSubscription<Action<string>>(
                h => OnResourceChanged += h,
                h => OnResourceChanged -= h,
                name => Printer.PrintResourceChanged(this, name)
            );
            RegisterSubscription<Action<string>>(
                h => OnResourceDepleted += h,
                h => OnResourceDepleted -= h,
                name => Printer.PrintResourceDepleted(this, name)
            );
        }
    }
}
