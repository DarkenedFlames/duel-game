namespace CBA
{
    public enum EffectAction {Apply, Remove}
    public enum TargetType {Self, Target}

    public class ModifiesEffects(
        Entity owner,
        Dictionary<(EffectAction, TargetType, Trigger), List<string>> triggeredEffects
    ) : Component(owner)
    {
        public Dictionary<(EffectAction Action, TargetType TargetType, Trigger Trigger), List<string>> TriggeredEffects { get; init; } = triggeredEffects;

        public override void Subscribe()
        {
            Entity wearer = World.Instance.GetPlayerOf(Owner);

            foreach (Trigger trigger in Enum.GetValues<Trigger>())
            {
                if (trigger == Trigger.None || !HasTrigger(trigger))
                    continue;

                switch (trigger)
                {
                    case Trigger.OnEquip:
                        var wearable = Owner.GetComponent<Wearable>();
                        TrackSubscription<Action<Entity>>(
                            h => wearable.OnEquipSuccess += h,
                            h => wearable.OnEquipSuccess -= h,
                            _ => Modify(wearer, null, trigger)
                        );
                        break;

                    case Trigger.OnUnequip:
                        wearable = Owner.GetComponent<Wearable>();
                        TrackSubscription<Action<Entity>>(
                            h => wearable.OnUnequipSuccess += h,
                            h => wearable.OnUnequipSuccess -= h,
                            _ => Modify(wearer, null, trigger)
                        );
                        break;

                    case Trigger.OnAdded:
                        TrackSubscription<Action<Entity>>(
                            h => World.Instance.OnEntityAdded += h,
                            h => World.Instance.OnEntityAdded -= h,
                            e => { if (e == Owner) Modify(wearer, null, trigger); }
                        );
                        break;

                    case Trigger.OnRemoved:
                        TrackSubscription<Action<Entity>>(
                            h => World.Instance.OnEntityRemoved += h,
                            h => World.Instance.OnEntityRemoved -= h,
                            e => { if (e == Owner) Modify(wearer, null, trigger); }
                        );
                        break;

                    case Trigger.OnUse:
                        var usable = Owner.GetComponent<Usable>();
                        TrackSubscription<Action<Entity, Entity>>(
                            h => usable.OnUseSuccess += h,
                            h => usable.OnUseSuccess -= h,
                            (_, target) => Modify(wearer, target, trigger)
                        );
                        break;

                    case Trigger.OnHit:
                        var hits = Owner.GetComponent<Hits>();
                        TrackSubscription<Action<Entity, Entity>>(
                            h => hits.OnHit += h,
                            h => hits.OnHit -= h,
                            (_, target) => Modify(wearer, target, trigger)
                        );
                        break;

                    case Trigger.OnCritical:
                        var deals = Owner.GetComponent<DealsDamage>();
                        TrackSubscription<Action<Entity, Entity>>(
                            h => deals.OnCritical += h,
                            h => deals.OnCritical -= h,
                            (_, target) => Modify(wearer, target, trigger)
                        );
                        break;

                    case Trigger.OnDamageDealt:
                        deals = Owner.GetComponent<DealsDamage>();
                        TrackSubscription<Action<Entity, Entity, int>>(
                            h => deals.OnDamageDealt += h,
                            h => deals.OnDamageDealt -= h,
                            (_, target, _) => Modify(wearer, target, trigger)
                        );
                        break;

                    case Trigger.OnTurnStart:
                        TrackSubscription<Action<Entity>>(
                            h => World.Instance.TurnManager.OnTurnStart += h,
                            h => World.Instance.TurnManager.OnTurnStart -= h,
                            player => { if (player == wearer) Modify(player, null, trigger); }
                        );
                        break;

                    case Trigger.OnTurnStartWhileEquipped:
                        TrackSubscription<Action<Entity>>(
                            h => World.Instance.TurnManager.OnTurnStart += h,
                            h => World.Instance.TurnManager.OnTurnStart -= h,
                            player =>
                            {
                                bool isEquipped = Owner.GetComponent<Wearable>().IsEquipped;
                                if (player == wearer && isEquipped) Modify(player, null, trigger);
                            }
                        );
                        break;

                    case Trigger.OnArmorSetCompleted:
                        var completes = Owner.GetComponent<CompletesItemSet>();
                        TrackSubscription<Action<Entity>>(
                            h => completes.OnArmorSetCompleted += h,
                            h => completes.OnArmorSetCompleted -= h,
                            _ => Modify(wearer, null, trigger)
                        );
                        break;

                    case Trigger.OnArmorSetBroken:
                        completes = Owner.GetComponent<CompletesItemSet>();
                        TrackSubscription<Action<Entity>>(
                            h => completes.OnArmorSetBroken += h,
                            h => completes.OnArmorSetBroken -= h,
                            _ => Modify(wearer, null, trigger)
                        );
                        break;
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
            foreach (Entity effect in allTargetEffects)
                World.Instance.RemoveEntity(effect);
        }

        private bool HasTrigger(Trigger trigger) => TriggeredEffects.Keys.Any(k => k.Trigger == trigger);
    }
}
