namespace CBA
{
    [Flags]
    public enum Trigger
    {
        None = 0,
        OnUse = 1 << 0,
        OnEquip = 1 << 1,
        OnUnequip = 1 << 2,
        OnAdded = 1 << 3,
        OnRemoved = 1 << 4,
        OnHit = 1 << 5,
        OnCritical = 1 << 6,
        OnDamageDealt = 1 << 7,
        OnTurnStart = 1 << 8,
        OnTurnStartWhileEquipped = 1 << 9,
        OnArmorSetCompleted = 1 << 10,
        OnArmorSetBroken = 1 << 11,
    }

    public enum ModificationType { Add, Multiply }

    public class ModifiesStats(
        Entity owner,
        Dictionary<(ModificationType, TargetType, Trigger), Dictionary<string, float>> statsByTrigger
        ) : Component(owner)
    {
        private readonly Dictionary<(ModificationType Type, TargetType TargetType, Trigger Trigger), Dictionary<string, float>> StatChanges = statsByTrigger;

        protected override void RegisterSubscriptions()
        {
            Entity wearer = World.GetPlayerOf(Owner);

            foreach (Trigger trigger in Enum.GetValues<Trigger>())
            {
                if (trigger == Trigger.None || !HasTrigger(trigger))
                    continue;

                RegisterTrigger(trigger, wearer);
            }
        }

        private void RegisterTrigger(Trigger trigger, Entity wearer)
        {
            switch (trigger)
            {
                case Trigger.OnEquip:
                    var wearable = Owner.GetComponent<Wearable>();
                    RegisterSubscription<Action<Entity>>(
                        h => wearable.OnEquipSuccess += h,
                        h => wearable.OnEquipSuccess -= h,
                        _ => Modify(wearer, null, Trigger.OnEquip)
                    );
                    break;

                case Trigger.OnUnequip:
                    wearable = Owner.GetComponent<Wearable>();
                    RegisterSubscription<Action<Entity>>(
                        h => wearable.OnUnequipSuccess += h,
                        h => wearable.OnUnequipSuccess -= h,
                        _ => Modify(wearer, null, Trigger.OnUnequip)
                    );
                    break;

                case Trigger.OnAdded:
                    RegisterSubscription<Action<Entity>>(
                        h => World.Instance.OnEntityAdded += h,
                        h => World.Instance.OnEntityAdded -= h,
                        e => { if (e == Owner) Modify(wearer, null, Trigger.OnAdded); }
                    );
                    break;

                case Trigger.OnRemoved:
                    RegisterSubscription<Action<Entity>>(
                        h => World.Instance.OnEntityRemoved += h,
                        h => World.Instance.OnEntityRemoved -= h,
                        e => { if (e == Owner) Modify(wearer, null, Trigger.OnRemoved); }
                    );
                    break;

                case Trigger.OnUse:
                    var usable = Owner.GetComponent<Usable>();
                    RegisterSubscription<Action<Entity, Entity>>(
                        h => usable.OnUseSuccess += h,
                        h => usable.OnUseSuccess -= h,
                        (_, target) => Modify(wearer, target, Trigger.OnUse)
                    );
                    break;

                case Trigger.OnHit:
                    var hits = Owner.GetComponent<Hits>();
                    RegisterSubscription<Action<Entity, Entity>>(
                        h => hits.OnHit += h,
                        h => hits.OnHit -= h,
                        (_, target) => Modify(wearer, target, Trigger.OnHit)
                    );
                    break;

                case Trigger.OnCritical:
                    var deals = Owner.GetComponent<DealsDamage>();
                    RegisterSubscription<Action<Entity, Entity>>(
                        h => deals.OnCritical += h,
                        h => deals.OnCritical -= h,
                        (_, target) => Modify(wearer, target, Trigger.OnCritical)
                    );
                    break;

                case Trigger.OnDamageDealt:
                    deals = Owner.GetComponent<DealsDamage>();
                    RegisterSubscription<Action<Entity, Entity, int>>(
                        h => deals.OnDamageDealt += h,
                        h => deals.OnDamageDealt -= h,
                        (_, target, _) => Modify(wearer, target, Trigger.OnDamageDealt)
                    );
                    break;

                case Trigger.OnArmorSetCompleted:
                    var completes = Owner.GetComponent<CompletesItemSet>();
                    RegisterSubscription<Action<Entity>>(
                        h => completes.OnArmorSetCompleted += h,
                        h => completes.OnArmorSetCompleted -= h,
                        _ => Modify(wearer, null, Trigger.OnArmorSetCompleted)
                    );
                    break;

                case Trigger.OnArmorSetBroken:
                    completes = Owner.GetComponent<CompletesItemSet>();
                    RegisterSubscription<Action<Entity>>(
                        h => completes.OnArmorSetBroken += h,
                        h => completes.OnArmorSetBroken -= h,
                        _ => Modify(wearer, null, Trigger.OnArmorSetBroken)
                    );
                    break;
            }
        }

        private bool HasTrigger(Trigger trigger) =>
            StatChanges.Keys.Any(k => k.Trigger == trigger);

        private void Modify(Entity source, Entity? target, Trigger trigger)
        {
            // Filter changes matching this trigger
            var matchingEntries = StatChanges
                .Where(kvp => kvp.Key.Trigger == trigger)
                .ToList();

            foreach (var ((type, targetType, _), changes) in matchingEntries)
            {
                Entity receiver = targetType switch
                {
                    TargetType.Self => source,
                    TargetType.Target => target ?? source,
                    _ => throw new InvalidOperationException($"Unsupported TargetType: {targetType}")
                };

                var stats = receiver.GetComponent<StatsComponent>();
                foreach (var (key, value) in changes)
                {
                    switch (type)
                    {
                        case ModificationType.Add:
                            stats.IncreaseBase(key, (int)value);
                            break;
                        case ModificationType.Multiply:
                            stats.IncreaseModifier(key, value);
                            break;
                    }
                }
            }
        }
    }
}