namespace CBA
{
    public class ResourcesComponent(Entity owner) : Component(owner)
    {
        public event Action<string>? OnResourceChanged;
        public event Action<string>? OnResourceDepleted;

        private readonly Dictionary<string, int> _values = [];

        private StatsComponent Stats() => Owner.GetComponent<StatsComponent>();

        private int ValidateResource(string name)
        {
            if (!_values.TryGetValue(name, out int value))
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
            if (!_values.TryGetValue(resourceName, out var value))
                return;

            int oldMax = resourceName switch
            {
                "Health" => Stats().Get("MaximumHealth"),
                "Stamina" => Stats().Get("MaximumStamina"),
                _ => newMax
            };

            float ratio = oldMax > 0 ? (float)value / oldMax : 1f;
            int newValue = Math.Clamp((int)(newMax * ratio), 0, newMax);

            _values[resourceName] = newValue;
            OnResourceChanged?.Invoke(resourceName);
        }

        public int Get(string name) => ValidateResource(name);

        public void Change(string name, int delta)
        {
            int value = ValidateResource(name);

            int max = name switch
            {
                "Health" => Stats().Get("MaximumHealth"),
                "Stamina" => Stats().Get("MaximumStamina"),
                _ => int.MaxValue
            };

            int newVal = Math.Clamp(value + delta, 0, max);
            _values[name] = newVal;

            OnResourceChanged?.Invoke(name);
            if (newVal <= 0) OnResourceDepleted?.Invoke(name);
        }

        public void Refill(string name)
        {
            int max = name switch
            {
                "Health" => Stats().Get("MaximumHealth"),
                "Stamina" => Stats().Get("MaximumStamina"),
                _ => int.MaxValue
            };

            _values[name] = max;
            OnResourceChanged?.Invoke(name);
        }

        protected override void RegisterSubscriptions()
        {
            _values["Health"] = Stats().Get("MaximumHealth");
            _values["Stamina"] = Stats().Get("MaximumStamina");

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
