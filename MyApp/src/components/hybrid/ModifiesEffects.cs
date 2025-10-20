namespace CBA
{
    public enum EffectAction
    {
        Apply,
        Remove
    }

    public enum TargetType
    {
        Self,
        Target
    }

    public class ModifiesEffects(
        Entity owner,
        Dictionary<(EffectAction, TargetType, Trigger), List<string>> triggeredEffects) : Component(owner)
    {
        public Dictionary<(EffectAction Action, TargetType TargetType, Trigger Trigger), List<string>> TriggeredEffects { get; init; } = triggeredEffects;

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
                            wearable.OnEquipSuccess += _ => Modify(wearer, null, Trigger.OnEquip);
                        else
                            wearable.OnUnequipSuccess += _ => Modify(wearer, null, Trigger.OnUnequip);
                        break;
                    }
                    case Trigger.OnAdded:
                    case Trigger.OnRemoved:
                    {
                        var target = World.Instance.GetPlayerOf(Owner);

                        if (trigger == Trigger.OnAdded)
                            World.Instance.OnEntityAdded += e => { if (e == Owner) Modify(target, null, trigger); };
                        else
                            World.Instance.OnEntityRemoved += e => { if (e == Owner) Modify(target, null, trigger); };
                        
                        break;
                    }
                    case Trigger.OnUse:
                        Owner.GetComponent<Usable>().OnUseSuccess += (_, target) =>
                        {
                            var user = World.Instance.GetPlayerOf(Owner);
                            Modify(user, target, trigger);
                        };
                        break;
                    case Trigger.OnHit:
                        Owner.GetComponent<Hits>().OnHit += (_, target) =>
                        {
                            var hitter = World.Instance.GetPlayerOf(Owner);
                            Modify(hitter, target, trigger);
                        };
                        break;
                    case Trigger.OnCritical:
                        Owner.GetComponent<DealsDamage>().OnCritical += (_, target) =>
                        {
                            var user = World.Instance.GetPlayerOf(Owner);
                            Modify(user, target, trigger);
                        };
                        break;
                    case Trigger.OnDamageDealt:
                        Owner.GetComponent<DealsDamage>().OnDamageDealt += (_, target, _) =>
                        {
                            var dealer = World.Instance.GetPlayerOf(Owner);
                            Modify(dealer, target, trigger);
                        };
                        break;
                    case Trigger.OnTurnStart:
                        World.Instance.TurnManager.OnTurnStart += turnTaker =>
                        {
                            var player = World.Instance.GetPlayerOf(Owner);
                            if (turnTaker == player) Modify(turnTaker, null, trigger);
                        };
                        break;
                    case Trigger.OnTurnStartWhileEquipped:
                        World.Instance.TurnManager.OnTurnStart += turnTaker =>
                        {
                            var player = World.Instance.GetPlayerOf(Owner);
                            bool isEquipped = Owner.GetComponent<Wearable>().IsEquipped;
                            if (turnTaker == player && isEquipped) Modify(turnTaker, null, trigger);
                        };
                        break;

                    case Trigger.OnArmorSetCompleted:
                    case Trigger.OnArmorSetBroken:
                    {
                        Entity wearer = World.Instance.GetPlayerOf(Owner);
                        if (trigger == Trigger.OnArmorSetCompleted)
                            Owner.GetComponent<CompletesItemSet>().OnArmorSetCompleted += _ => Modify(wearer, null, trigger);
                        else
                            Owner.GetComponent<CompletesItemSet>().OnArmorSetBroken += _ => Modify(wearer, null, trigger);
                        break;
                    }
                }
            }
        }
        private void Modify(Entity source, Entity? target, Trigger trigger)
        {
            var matchingEntries = TriggeredEffects
                .Where(kvp => kvp.Key.Trigger == trigger)
                .ToList();

            foreach (var ((action, targetType, _), effects) in matchingEntries)
            {
                // Determine who should actually receive the effect
                Entity receiver = targetType switch
                {
                    TargetType.Self => source,
                    TargetType.Target => target ?? source,
                    _ => source
                };

                foreach (var effectTypeId in effects)
                {
                    if (action == EffectAction.Apply)
                        EffectFactory.ApplyEffect(effectTypeId, receiver);
                    else
                        RemoveEffect(effectTypeId, receiver);
                }
            }
        }
        private static void RemoveEffect(string effectTypeId, Entity target)
        {
            List<Entity> allTargetEffects = [..World.Instance.GetAllForPlayer<Entity>(target, EntityCategory.Effect, effectTypeId)];
            foreach (Entity effect in allTargetEffects) World.Instance.RemoveEntity(effect);
        }
        private bool HasTrigger(Trigger trigger)
        {
            return TriggeredEffects.Keys.Any(k => k.Trigger == trigger);
        }
    }
}
