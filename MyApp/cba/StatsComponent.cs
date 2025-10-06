using System;
using System.Collections.Generic;

namespace MyCBA
{
    public class StatsComponent : Component
    {
        private readonly Dictionary<string, (int BaseValue, float Multiplier)> _stats = new();

        public event Action<string, int>? OnStatChanged;

        public void InitializeDefaultStats()
        {
            AddStat("MaximumHealth", 100, 1.0f);
            AddStat("MaximumStamina", 100, 1.0f);
            AddStat("Armor", 50, 1.0f);
            AddStat("Shield", 50, 1.0f);
            AddStat("Critical", 0, 1.0f);
            AddStat("Dodge", 0, 1.0f);
            AddStat("Peer", 0, 1.0f);
            AddStat("Luck", 0, 1.0f);
        }

        public void AddStat(string name, int baseValue, float multiplier = 1.0f)
        {
            _stats[name] = (baseValue, multiplier);
            OnStatChanged?.Invoke(name, GetStatValue(name));
        }

        public int GetStatValue(string name)
        {
            if (!_stats.TryGetValue(name, out var data))
                return 0;

            return (int)(data.BaseValue * data.Multiplier);
        }

        public void ModifyBaseValue(string name, int delta)
        {
            if (!_stats.ContainsKey(name))
                return;

            var stat = _stats[name];
            stat.BaseValue += delta;
            _stats[name] = stat;
            OnStatChanged?.Invoke(name, GetStatValue(name));
        }

        public void ModifyMultiplier(string name, float percent)
        {
            if (!_stats.ContainsKey(name))
                return;

            var stat = _stats[name];
            stat.Multiplier *= percent;
            _stats[name] = stat;
            OnStatChanged?.Invoke(name, GetStatValue(name));
        }

        public bool HasStat(string name) => _stats.ContainsKey(name);

        public IReadOnlyDictionary<string, (int BaseValue, float Multiplier)> AllStats => _stats;
    }
}
