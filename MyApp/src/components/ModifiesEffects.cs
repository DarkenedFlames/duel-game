namespace CBA
{
    [Flags]
    public enum EffectTrigger
    {
        None      = 0,
        OnUse     = 1 << 0,
        OnEquip   = 1 << 1,
        OnUnequip = 1 << 2
    }

    public class ModifiesEffects(Entity owner) : Component(owner)
    {
        public Dictionary<EffectTrigger, List<string>> TriggeredEffects { get; } = new();
        public event Action<Entity, Entity, string>? OnEffectApplied;

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Item)
                throw new InvalidOperationException($"{Owner.Id} was given an invalid Component: ModifiesEffects.");

            if (!Owner.HasComponent<Usable>() || !Owner.HasComponent<Wearable>())
                throw new InvalidOperationException($"Component Missing a Dependency: (Owner: {Owner.Id}, Component: ModifiesEffects, Dependency: Usable or Wearable.");
        }
        public override void Subscribe()
        {
            if (Owner.HasComponent<Usable>())
                Owner.GetComponent<Usable>().OnUseSuccess += (_, target) => ApplyByTrigger(EffectTrigger.OnUse, target);

            if (Owner.HasComponent<Wearable>())
            {
                Wearable wearable = Owner.GetComponent<Wearable>();
                Entity wearer = Owner.GetComponent<ItemData>().PlayerEntity;
                wearable.OnEquipSuccess += _ => { ApplyByTrigger(EffectTrigger.OnEquip, wearer); };
                wearable.OnUnequipSuccess += _ => { ApplyByTrigger(EffectTrigger.OnUnequip, wearer); };   
            }
        }
        private void ApplyByTrigger(EffectTrigger trigger, Entity target)
        {
            if (!TriggeredEffects.TryGetValue(trigger, out List<string>? effects))
                throw new InvalidOperationException($"[{Owner.Id}] Required trigger missing in TriggeredEffects: {nameof(trigger)}.");
            
            foreach (string effectTypeId in effects)
            {
                if (trigger == EffectTrigger.OnUnequip) RemoveEffect(effectTypeId, target);
                else ApplyEffect(effectTypeId, target);
            }
        }
        private void ApplyEffect(string effectTypeId, Entity target)
        {
            EffectFactory.ApplyEffect(effectTypeId, target);
            OnEffectApplied?.Invoke(Owner, target, effectTypeId);
        }
        private static void RemoveEffect(string effectTypeId, Entity target)
        {
            foreach (Entity e in World.Instance.GetById(EntityCategory.Effect, effectTypeId))
                if (e.GetComponent<EffectData>().PlayerEntity == target)
                    World.Instance.RemoveEntity(e);
        }
    }
}
