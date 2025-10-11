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
        OnUnequip = 1 << 2,
        OnApply = 1 << 3,   // Effect applied (entity added)
        OnRemove = 1 << 4   // Effect removed (entity removed)
    }

    public class ModifiesStats(Entity owner, ModifiesStatsTrigger triggers) : Component(owner)
    {
        public Dictionary<string, float> StatModifiers { get; } = [];
        public Dictionary<string, int> StatAdditions { get; } = [];
        public Dictionary<string, float> ResourceModifiers { get; } = [];
        public Dictionary<string, int> ResourceAdditions { get; } = [];

        public ModifiesStatsTrigger Triggers { get; } = triggers;

        public event Action<Entity, Entity>? OnStatsModified;

        public override void Subscribe()
        {
            // Item logic
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
                        wearable.OnEquipSuccess += item =>
                        {
                            var user = item.GetComponent<ItemData>()?.PlayerEntity;
                            if (user != null) Apply(user);
                        };

                    if (Triggers.HasFlag(ModifiesStatsTrigger.OnUnequip))
                        wearable.OnUnequipSuccess += item =>
                        {
                            var user = item.GetComponent<ItemData>()?.PlayerEntity;
                            if (user != null) Remove(user);
                        };

                                
                }
            }

            // Effect logic
            if (Triggers.HasFlag(ModifiesStatsTrigger.OnApply))
            {
                World.Instance.OnEntityAdded += entity =>
                {
                    var effectData = entity.GetComponent<EffectData>();
                    if (effectData != null && entity == Owner)
                        Apply(effectData.PlayerEntity);
                };
            }

            if (Triggers.HasFlag(ModifiesStatsTrigger.OnRemove))
            {
                World.Instance.OnEntityRemoved += entity =>
                {
                    var effectData = entity.GetComponent<EffectData>();
                    if (effectData != null && entity == Owner)
                        Remove(effectData.PlayerEntity);
                };
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
                    resources.ChangeMultiplier(kvp.Key, 1f / kvp.Value); // undo factor
            }

            OnStatsModified?.Invoke(Owner, target);
        }
    }
}