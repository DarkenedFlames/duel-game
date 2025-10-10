using System;
using System.Collections.Generic;

namespace CBA
{
    // Defines which entity the effect should target
    public enum EffectTargetType
    {
        User,   // Entity using or equipping the item
        Target  // Entity being acted upon
    }

    // Stores effect info for each trigger
    public class EffectInfo
    {
        public string EffectName { get; init; } = string.Empty;
        public EffectTargetType TargetType { get; init; } = EffectTargetType.Target;
    }

    // Flags for item triggers
    [Flags]
    public enum EffectTrigger
    {
        None      = 0,
        OnUse     = 1 << 0,
        OnEquip   = 1 << 1,
        OnUnequip = 1 << 2
        // Future triggers can be added here (e.g., OnAttack, OnDefend)
    }

    public class ModifiesEffects(Entity owner) : Component(owner)
    {
        // Maps each trigger type to a list of associated effects
        public Dictionary<EffectTrigger, List<EffectInfo>> TriggeredEffects { get; } = [];

        public event Action<Entity, Entity, EffectInfo>? OnEffectApplied;

        protected override void Subscribe()
        {
            var usable = Owner.GetComponent<Usable>();
            var wearable = Owner.GetComponent<Wearable>();

            // --- On Use ---
            if (usable != null)
            {
                usable.OnUseSuccess += (item, target) =>
                {
                    ApplyByTrigger(EffectTrigger.OnUse, item, target);
                };
            }

            // --- On Equip ---
            if (wearable != null)
            {
                wearable.OnEquipSuccess += item =>
                {
                    ApplyByTrigger(EffectTrigger.OnEquip, item, item);
                };

                // --- On Unequip ---
                wearable.OnUnequipSuccess += item =>
                {
                    ApplyByTrigger(EffectTrigger.OnUnequip, item, item);
                };
            }
        }

        private void ApplyByTrigger(EffectTrigger trigger, Entity item, Entity target)
        {
            if (!TriggeredEffects.TryGetValue(trigger, out var effects)) return;

            var user = item.GetComponent<ItemData>()?.PlayerEntity;
            if (user == null) return;

            foreach (var effect in effects)
            {
                if (trigger == EffectTrigger.OnUnequip)
                    RemoveEffect(effect, target);
                else
                    ApplyEffect(effect, user, target);
            }
        }

        private void ApplyEffect(EffectInfo effect, Entity user, Entity target)
        {
            var actualTarget = effect.TargetType == EffectTargetType.User ? user : target;

            EffectFactory.ApplyEffect(effect.EffectName, actualTarget);

            OnEffectApplied?.Invoke(Owner, actualTarget, effect);
        }

        private void RemoveEffect(EffectInfo effect, Entity target)
        {
            foreach (Entity e in World.Instance.GetEntitiesWith<EffectData>())
            {
                var effectData = e.GetComponent<EffectData>();
                if (effectData != null &&
                    effectData.Name == effect.EffectName &&
                    effectData.PlayerEntity == target)
                {
                    World.Instance.RemoveEntity(e);
                }
            }
        }
    }
}
