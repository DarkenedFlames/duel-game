namespace CBA
{
    public enum EffectAction
    {
        Apply,
        Remove
    }

    public class ModifiesEffects(
        Entity owner,
        Dictionary<(EffectAction, Trigger), List<string>> triggeredEffects) : Component(owner)
    {
        public Dictionary<(EffectAction Action, Trigger Trigger), List<string>> TriggeredEffects { get; } = triggeredEffects;

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Item)
                throw new InvalidOperationException($"{Owner.Id} was given an invalid Component: ModifiesEffects.");

            if (!Owner.HasComponent<Usable>() && !Owner.HasComponent<Wearable>())
                throw new InvalidOperationException($"Component Missing a Dependency: (Owner: {Owner.Id}, Component: ModifiesEffects, Dependency: Usable or Wearable.");
        }
        public override void Subscribe()
        {
            foreach (Trigger trigger in Enum.GetValues<Trigger>())
            {
                if (trigger == Trigger.None) continue;
                if (!HasTrigger(trigger)) continue;

                switch (trigger)
                {
                    case Trigger.OnEquip:
                    case Trigger.OnUnequip:
                    {
                        var wearable = Owner.GetComponent<Wearable>();
                        var wearer = World.Instance.GetPlayerOf(Owner);
                        if (trigger == Trigger.OnEquip)
                            wearable.OnEquipSuccess += _ => Modify(wearer, Trigger.OnEquip);
                        else
                            wearable.OnUnequipSuccess += _ => Modify(wearer, Trigger.OnUnequip);
                        break;
                    }
                    case Trigger.OnAdded:
                    case Trigger.OnRemoved:
                    {
                        var target = World.Instance.GetPlayerOf(Owner);

                        if (trigger == Trigger.OnAdded)
                            World.Instance.OnEntityAdded += e => { if (e == Owner) Modify(target, trigger); };
                        else
                            World.Instance.OnEntityRemoved += e => { if (e == Owner) Modify(target, trigger); };
                        
                        break;
                    }
                    case Trigger.OnUse:
                        Owner.GetComponent<Usable>().OnUseSuccess += (_, target) => Modify(target, trigger);
                        break;
                    case Trigger.OnHit:
                        Owner.GetComponent<Hits>().OnHit += (_, target) => Modify(target, trigger);
                        break;
                    case Trigger.OnCritical:
                        Owner.GetComponent<DealsDamage>().OnCritical += (_, target) => Modify(target, trigger);
                        break;
                    case Trigger.OnDamageDealt:
                        Owner.GetComponent<DealsDamage>().OnDamageDealt += (_, target, _) => Modify(target, trigger);
                        break;
                }
            }
        }
        private void Modify(Entity target, Trigger trigger)
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
        private bool HasTrigger(Trigger trigger)
        {
            return TriggeredEffects.Keys.Any(k => k.Trigger == trigger);
        }
    
    }
}
