namespace CBA
{
    [Flags]
    public enum EffectTrigger
    {
        None = 0,
        OnUse = 1 << 0,
        OnEquip = 1 << 1,
        OnUnequip = 1 << 2,
        OnHit = 1 << 3,
        OnCritical = 1 << 4
    }

    public enum EffectAction
    {
        Apply,
        Remove
    }

    public class ModifiesEffects(Entity owner) : Component(owner)
    {
        public Dictionary<(EffectAction Action, EffectTrigger Trigger), List<string>> TriggeredEffects { get; } = [];

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Item)
                throw new InvalidOperationException($"{Owner.Id} was given an invalid Component: ModifiesEffects.");

            if (!Owner.HasComponent<Usable>() && !Owner.HasComponent<Wearable>())
                throw new InvalidOperationException($"Component Missing a Dependency: (Owner: {Owner.Id}, Component: ModifiesEffects, Dependency: Usable or Wearable.");
        }
        public override void Subscribe()
        {
            if(HasTrigger(EffectTrigger.OnUse)) 
                Owner.GetComponent<Usable>().OnUseSuccess += (_, target) => ExecuteByTrigger(EffectTrigger.OnUse, target);

            if (HasTrigger(EffectTrigger.OnEquip))
            {
                Wearable wearable = Owner.GetComponent<Wearable>();
                Entity wearer = World.Instance.GetPlayerOf(Owner);
                wearable.OnEquipSuccess += _ => ExecuteByTrigger(EffectTrigger.OnEquip, wearer);
            }
            if(HasTrigger(EffectTrigger.OnUnequip))
            {
                Wearable wearable = Owner.GetComponent<Wearable>();
                Entity wearer = World.Instance.GetPlayerOf(Owner);
                wearable.OnUnequipSuccess += _ => ExecuteByTrigger(EffectTrigger.OnUnequip, wearer);
            }
            if (HasTrigger(EffectTrigger.OnHit))
                Owner.GetComponent<Hits>().OnHit += (_, target) => ExecuteByTrigger(EffectTrigger.OnHit, target);

            if (HasTrigger(EffectTrigger.OnCritical))
                Owner.GetComponent<DealsDamage>().OnCritical += (_, target) => ExecuteByTrigger(EffectTrigger.OnCritical, target);
        }
        private void ExecuteByTrigger(EffectTrigger trigger, Entity target)
        {
            var matchingEntries = TriggeredEffects
                .Where(kvp => kvp.Key.Trigger == trigger)
                .ToList();

            foreach (var ((action, _), effects ) in matchingEntries)
            {
                foreach (var effectTypeId in effects)
                {
                    if (action == EffectAction.Apply)
                        EffectFactory.ApplyEffect(effectTypeId, target);
                    else
                        RemoveEffect(effectTypeId, target);

                }
            }
        }
        private static void RemoveEffect(string effectTypeId, Entity target)
        {
            IEnumerable<Entity> allTargetEffects = World.Instance.GetAllForPlayer<Entity>(target, EntityCategory.Effect, effectTypeId);
            foreach (Entity effect in allTargetEffects) World.Instance.RemoveEntity(effect);
        }
        private bool HasTrigger(EffectTrigger trigger)
        {
            return TriggeredEffects.Keys.Any(k => k.Trigger == trigger);
        }
    
    }
}
