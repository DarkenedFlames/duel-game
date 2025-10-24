using System.ComponentModel.DataAnnotations;

namespace CBA
{
    public class StatsComponent(Entity owner) : Component(owner)
    {
        public event Action<string>? OnStatChanged;

        private readonly Dictionary<string, (int Base, float Modifier)> _values = new()
        {
            // Resource Stats
            {"MaximumHealth",  (100, 1.0f)},
            {"MaximumStamina", (100, 1.0f)},
            {"Healing",        (100, 1.0f)}, // Represents the percentage of each heal that the player actually receives. Min = .25
            {"Stimming",       (100, 1.0f)}, // Represents the percentage of each stim that the player actually receives. Min = .25

            // Receiving Items
            {"Luck",  (0, 1.0f)}, // Represents the chance of getting a second item, scales hyperbolically
            {"Steal", (0, 1.0f)}, // Represents the chance of sealing an item, scales hyperbolically

            // Using Items
            { "ConsumableCost", (100, 1.0f) }, // Represents the percentage of each consumable cost that the player expends, Min = .25
            { "WeaponCost", (100, 1.0f) },     // Represents the percentage of each weapon cost that the player expends, Min = .25

            // Dealing Damage
            { "Attack", (100, 1.0f) },    // Represents the percentage of each damage instance that the player deals, Min = .25
            { "Critical", (0, 1.0f) },    // Chance-Based stat
            { "Accuracy", (100, 1.0f) },  // Represents the percentage chance to hit, Min = .25
            { "Precision", (100, 1.0f) }, // Represents the percentage of the attack damage that critical strikes deal on top, Min = 0

            // Receiving Damage
            { "Armor", (50, 1.0f) }, // Percentage Stat, scales hyperbolically
            { "Shield", (50, 1.0f) }, // Percentage Stat, scales hyperbolically
            { "Dodge", (0, 1.0f) }, // Chance-Based stat, scales hyperbolically
        };

        public bool HasStat(string name) => _values.ContainsKey(name);
        public int Get(string name)
        {
            ValidateStat(name);
            (int Base, float Modifier) = _values[name];
            return (int)(Base * Modifier);
        }
        public float GetHyperbolic(string name) => Get(name) / (Get(name) + 100f);
        public float GetLinearClamped(string name, float minimum) => Math.Max(minimum, Get($"{name}") / 100f);

        public void IncreaseBase(string name, int delta)
        {
            ValidateStat(name);
            (int Base, float Modifier) = _values[name];
            _values[name] = (Base + delta, Modifier);
            OnStatChanged?.Invoke(name);
        }
        public void IncreaseModifier(string name, float factor)
        {
            ValidateStat(name);
            (int Base, float Modifier) = _values[name];
            _values[name] = (Base, Modifier * factor);
            OnStatChanged?.Invoke(name);
        }
        protected override void RegisterSubscriptions()
        {
            RegisterSubscription<Action<string>>(
                h => OnStatChanged += h,
                h => OnStatChanged -= h,
                name => Printer.PrintStatChanged(this, name)
            );
        }
        private void ValidateStat(string name)
        {
            if (!HasStat(name)) throw new ArgumentException($"Stat '{name}' does not exist.");
        }


    }
}
