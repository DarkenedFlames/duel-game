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
            Usable? usable = Owner.GetComponent<Usable>();
            Wearable? wearable = Owner.GetComponent<Wearable>();

            // At least one of these must exist
            Helper.NotAllAreNull(
                "ModifiesEffects.Subscribe(): Expected either Usable or Wearable on entity " + Owner,
                usable,
                wearable
            );

            // --- Item use ---
            usable?.OnUseSuccess += (_, target) =>
                ApplyByTrigger(EffectTrigger.OnUse, target);

            // --- Equip / Unequip logic ---
            if (wearable is not null)
            {
                wearable.OnEquipSuccess += item =>
                {
                    ItemData itemData = Helper.ThisIsNotNull(
                        item.GetComponent<ItemData>(),
                        "ItemData missing on equipped item."
                    );

                    Entity wearer = Helper.ThisIsNotNull(
                        itemData.PlayerEntity,
                        "PlayerEntity missing on equipped item."
                    );

                    ApplyByTrigger(EffectTrigger.OnEquip, wearer);
                };

                wearable.OnUnequipSuccess += item =>
                {
                    ItemData itemData = Helper.ThisIsNotNull(
                        item.GetComponent<ItemData>(),
                        "ItemData missing on unequipped item."
                    );

                    Entity wearer = Helper.ThisIsNotNull(
                        itemData.PlayerEntity,
                        "PlayerEntity missing on unequipped item."
                    );

                    ApplyByTrigger(EffectTrigger.OnUnequip, wearer);
                };
            }
        }

        private void ApplyByTrigger(EffectTrigger trigger, Entity target)
        {
            if (!TriggeredEffects.TryGetValue(trigger, out List<string>? effects))
                return;

            foreach (string effectName in effects)
            {
                if (trigger == EffectTrigger.OnUnequip)
                    RemoveEffect(effectName, target);
                else
                    ApplyEffect(effectName, target);
            }
        }

        private void ApplyEffect(string effectName, Entity target)
        {
            EffectFactory.ApplyEffect(effectName, target);
            OnEffectApplied?.Invoke(Owner, target, effectName);
        }

        private static void RemoveEffect(string effectName, Entity target)
        {
            foreach (Entity e in World.Instance.GetEntitiesWith<EffectData>())
            {
                EffectData effectData = Helper.ThisIsNotNull(
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
