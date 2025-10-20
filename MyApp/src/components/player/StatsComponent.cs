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
            { "Luck", (0, 1.0f) },
            { "Steal", (0, 1.0f) },
            { "Damage", (0, 1.0f) },
            { "Resistance", (0, 1.0f) },
            { "PotionCost", (0, 1.0f)},
            { "PotionStrength", (0, 1.0f) }

        };

        public bool HasStat(string name) => _values.ContainsKey(name);
        private void ValidateStat(string name)
        {
            if (!HasStat(name)) throw new ArgumentException($"Stat '{name}' does not exist.");
        }

        public int Get(string name)
        {
            ValidateStat(name);
            (int Base, float Modifier) = _values[name];
            return (int)(Base * Modifier);
        }
        public float GetHyperbolic(string name)
        {
            ValidateStat(name);
            float hyperbolic = Get(name);
            hyperbolic /= hyperbolic + 100;
            return hyperbolic;
        }

        public void IncreaseBase(string name, int delta)
        {
            ValidateStat(name);
            (int Base, float Modifier) = _values[name];
            _values[name] = (Base + delta, Modifier);
            OnStatChanged?.Invoke(name);
        }
        public void DecreaseBase(string name, int delta) => IncreaseBase(name, -delta);

        public void IncreaseModifier(string name, float factor)
        {
            ValidateStat(name);
            (int Base, float Modifier) = _values[name];
            _values[name] = (Base, Modifier * factor);
            OnStatChanged?.Invoke(name);
        }
        public void DecreaseModifier(string name, float factor) => IncreaseModifier(name, 1f / factor);

        public override void Subscribe()
        {
            OnStatChanged += name => Printer.PrintStatChanged(this, name);
        }
    }
}
