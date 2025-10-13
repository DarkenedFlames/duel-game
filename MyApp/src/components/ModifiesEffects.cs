namespace CBA
{
    [Flags]
    public enum EffectTrigger
    {
        None      = 0,
        OnUse     = 1 << 0,
        OnEquip   = 1 << 1,
        OnUnequip = 1 << 2
        // Future triggers: OnAttack, OnDefend, etc.
    }

    public class ModifiesEffects(Entity owner) : Component(owner)
    {
        // Map triggers to list of effect names
        public Dictionary<EffectTrigger, List<string>> TriggeredEffects { get; } = new();

        public event Action<Entity, Entity, string>? OnEffectApplied;

        public override void Subscribe()
        {
            var usable = Owner.GetComponent<Usable>();
            var wearable = Owner.GetComponent<Wearable>();

            // At least one of these must exist
            Helper.NotAllAreNull(
                "ModifiesEffects.Subscribe(): Expected either Usable or Wearable on entity " + Owner,
                usable,
                wearable
            );

            // --- Item use ---
            usable?.OnUseSuccess += (item, target) =>
                ApplyByTrigger(EffectTrigger.OnUse, item, target);

            // --- Equip / Unequip logic ---
            if (wearable is not null)
            {
                wearable.OnEquipSuccess += item =>
                {
                    var itemData = Helper.ThisIsNotNull(
                        item.GetComponent<ItemData>(),
                        "ItemData missing on equipped item."
                    );

                    var wearer = Helper.ThisIsNotNull(
                        itemData.PlayerEntity,
                        "PlayerEntity missing on equipped item."
                    );

                    ApplyByTrigger(EffectTrigger.OnEquip, item, wearer);
                };

                wearable.OnUnequipSuccess += item =>
                {
                    var itemData = Helper.ThisIsNotNull(
                        item.GetComponent<ItemData>(),
                        "ItemData missing on unequipped item."
                    );

                    var wearer = Helper.ThisIsNotNull(
                        itemData.PlayerEntity,
                        "PlayerEntity missing on unequipped item."
                    );

                    ApplyByTrigger(EffectTrigger.OnUnequip, item, wearer);
                };
            }
        }

        private void ApplyByTrigger(EffectTrigger trigger, Entity item, Entity target)
        {
            if (!TriggeredEffects.TryGetValue(trigger, out var effects))
                return;

            // Ensure target and user are valid
            var itemData = Helper.ThisIsNotNull(
                item.GetComponent<ItemData>(),
                "ItemData missing in ModifiesEffects.ApplyByTrigger"
            );

            var user = Helper.ThisIsNotNull(
                itemData.PlayerEntity,
                "PlayerEntity missing for item in ModifiesEffects.ApplyByTrigger"
            );

            var actualTarget = Helper.ThisIsNotNull(
                target,
                "Target cannot be null in ModifiesEffects.ApplyByTrigger"
            );

            foreach (var effectName in effects)
            {
                if (trigger == EffectTrigger.OnUnequip)
                    RemoveEffect(effectName, actualTarget);
                else
                    ApplyEffect(effectName, user, actualTarget);
            }
        }

        private void ApplyEffect(string effectName, Entity user, Entity target)
        {
            var actualTarget = target;
            EffectFactory.ApplyEffect(effectName, actualTarget);
            OnEffectApplied?.Invoke(Owner, actualTarget, effectName);
        }

        private void RemoveEffect(string effectName, Entity target)
        {
            foreach (var e in World.Instance.GetEntitiesWith<EffectData>())
            {
                var effectData = Helper.ThisIsNotNull(
                    e.GetComponent<EffectData>(),
                    $"Effect entity {e} missing EffectData."
                );

                if (effectData.Name == effectName &&
                    effectData.PlayerEntity == target)
                {
                    World.Instance.RemoveEntity(e);
                }
            }
        }
    }
}
