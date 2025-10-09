using System;
using System.Collections.Generic;

namespace CBA
{
    [Flags]
    public enum ModifiesStatsTrigger
    {
        None = 0,
        OnUse = 1 << 0,
        OnEquip = 1 << 1,
        OnUnequip = 1 << 2
    }

    public class ModifiesStats : Component
    {
        public Dictionary<string, float> StatModifiers { get; } = new();
        public Dictionary<string, int> StatAdditions { get; } = new();
        public Dictionary<string, float> ResourceModifiers { get; } = new();
        public Dictionary<string, int> ResourceAdditions { get; } = new();

        public ModifiesStatsTrigger Triggers { get; }

        public event Action<Entity, Entity>? OnStatsModified;

        public ModifiesStats(Entity owner, ModifiesStatsTrigger triggers) : base(owner)
        {
            Triggers = triggers;
        }

        protected override void Subscribe()
        {
            if (Triggers.HasFlag(ModifiesStatsTrigger.OnUse))
            {
                var usable = Owner.GetComponent<Usable>();
                if (usable != null)
                    usable.OnUseSuccess += (user, target) => Apply(target);
            }

            if (Triggers.HasFlag(ModifiesStatsTrigger.OnEquip) || Triggers.HasFlag(ModifiesStatsTrigger.OnUnequip))
            {
                var wearable = Owner.GetComponent<Wearable>();
                if (wearable != null)
                {
                    if (Triggers.HasFlag(ModifiesStatsTrigger.OnEquip))
                        wearable.OnEquipSuccess += user => Apply(user);

                    if (Triggers.HasFlag(ModifiesStatsTrigger.OnUnequip))
                        wearable.OnUnequipSuccess += user => Remove(user);
                }
            }
        }

        private void Apply(Entity target)
        {
            var stats = target.GetComponent<StatsComponent>();
            var resources = target.GetComponent<ResourcesComponent>();

            if (stats != null)
            {
                foreach (var kvp in StatAdditions)
                    stats.IncreaseBase(kvp.Key, kvp.Value);

                foreach (var kvp in StatModifiers)
                    stats.IncreaseModifier(kvp.Key, kvp.Value);
            }

            if (resources != null)
            {
                foreach (var kvp in ResourceAdditions)
                    resources.Change(kvp.Key, kvp.Value);

                foreach (var kvp in ResourceModifiers)
                    resources.ChangeMultiplier(kvp.Key, kvp.Value);
            }

            OnStatsModified?.Invoke(Owner, target);
        }

        private void Remove(Entity target)
        {
            var stats = target.GetComponent<StatsComponent>();
            var resources = target.GetComponent<ResourcesComponent>();

            if (stats != null)
            {
                foreach (var kvp in StatAdditions)
                    stats.DecreaseBase(kvp.Key, kvp.Value);

                foreach (var kvp in StatModifiers)
                    stats.DecreaseModifier(kvp.Key, kvp.Value);
            }

            if (resources != null)
            {
                foreach (var kvp in ResourceAdditions)
                    resources.Change(kvp.Key, -kvp.Value);

                foreach (var kvp in ResourceModifiers)
                    resources.ChangeMultiplier(kvp.Key, 1f / kvp.Value); // inverse factor to undo
            }

            OnStatsModified?.Invoke(Owner, target);
        }
    }
}
