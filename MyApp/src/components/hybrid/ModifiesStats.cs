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

    public enum ModificationType
    {
        Add,
        Multiply
    }

    public class ModifiesStats(
        Entity owner,
        Dictionary<(Trigger, ModificationType), Dictionary<string, float>>? StatsByTrigger = null,
        Dictionary<(Trigger, ModificationType), Dictionary<string, float>>? ResourcesByTrigger = null
    ) : Component(owner)
    {
        public Dictionary<(Trigger Trigger, ModificationType Type), Dictionary<string, float>>? StatChanges { get; } = StatsByTrigger;
        public Dictionary<(Trigger Trigger, ModificationType Type), Dictionary<string, float>>? ResourceChanges { get; } = ResourcesByTrigger;

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
                            _ => Modify(wearer, Trigger.OnEquip)
                        );
                        break;

                    case Trigger.OnUnequip:
                        wearable = Owner.GetComponent<Wearable>();
                        TrackSubscription<Action<Entity>>(
                            h => wearable.OnUnequipSuccess += h,
                            h => wearable.OnUnequipSuccess -= h,
                            _ => Modify(wearer, Trigger.OnUnequip)
                        );
                        break;

                    case Trigger.OnAdded:
                        TrackSubscription<Action<Entity>>(
                            h => World.Instance.OnEntityAdded += h,
                            h => World.Instance.OnEntityAdded -= h,
                            e => { if (e == Owner) Modify(wearer, Trigger.OnAdded); }
                        );
                        break;

                    case Trigger.OnRemoved:
                        TrackSubscription<Action<Entity>>(
                            h => World.Instance.OnEntityRemoved += h,
                            h => World.Instance.OnEntityRemoved -= h,
                            e => { if (e == Owner) Modify(wearer, Trigger.OnRemoved); }
                        );
                        break;

                    case Trigger.OnUse:
                        var usable = Owner.GetComponent<Usable>();
                        TrackSubscription<Action<Entity, Entity>>(
                            h => usable.OnUseSuccess += h,
                            h => usable.OnUseSuccess -= h,
                            (_, target) => Modify(target, Trigger.OnUse)
                        );
                        break;

                    case Trigger.OnHit:
                        var hits = Owner.GetComponent<Hits>();
                        TrackSubscription<Action<Entity, Entity>>(
                            h => hits.OnHit += h,
                            h => hits.OnHit -= h,
                            (_, target) => Modify(target, Trigger.OnHit)
                        );
                        break;

                    case Trigger.OnCritical:
                        var deals = Owner.GetComponent<DealsDamage>();
                        TrackSubscription<Action<Entity, Entity>>(
                            h => deals.OnCritical += h,
                            h => deals.OnCritical -= h,
                            (_, target) => Modify(target, Trigger.OnCritical)
                        );
                        break;

                    case Trigger.OnDamageDealt:
                        deals = Owner.GetComponent<DealsDamage>();
                        TrackSubscription<Action<Entity, Entity, int>>(
                            h => deals.OnDamageDealt += h,
                            h => deals.OnDamageDealt -= h,
                            (_, target, _) => Modify(target, Trigger.OnDamageDealt)
                        );
                        break;

                    case Trigger.OnArmorSetCompleted:
                        var completes = Owner.GetComponent<CompletesItemSet>();
                        TrackSubscription<Action<Entity>>(
                            h => completes.OnArmorSetCompleted += h,
                            h => completes.OnArmorSetCompleted -= h,
                            _ => Modify(wearer, Trigger.OnArmorSetCompleted)
                        );
                        break;

                    case Trigger.OnArmorSetBroken:
                        completes = Owner.GetComponent<CompletesItemSet>();
                        TrackSubscription<Action<Entity>>(
                            h => completes.OnArmorSetBroken += h,
                            h => completes.OnArmorSetBroken -= h,
                            _ => Modify(wearer, Trigger.OnArmorSetBroken)
                        );
                        break;
                }
            }
        }

        private bool HasTrigger(Trigger trigger)
        {
            return (StatChanges?.Keys.Any(k => k.Trigger == trigger) ?? false) ||
                   (ResourceChanges?.Keys.Any(k => k.Trigger == trigger) ?? false);
        }

        private void Modify(Entity target, Trigger trigger)
        {
            if (target.Id.Category != EntityCategory.Player)
                throw new InvalidOperationException($"[{Owner.Id}] ModifiesStats.Modify was passed a non-player target.");

            var stats = target.GetComponent<StatsComponent>();
            var resources = target.GetComponent<ResourcesComponent>();

            // --- Stats ---
            if (StatChanges != null)
            {
                foreach (var ((trig, type), changes) in StatChanges)
                {
                    if (trig != trigger) continue;

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

            // --- Resources ---
            if (ResourceChanges != null)
            {
                foreach (var ((trig, type), changes) in ResourceChanges)
                {
                    if (trig != trigger) continue;

                    foreach (var (key, value) in changes)
                    {
                        switch (type)
                        {
                            case ModificationType.Add:
                                resources.Change(key, (int)value);
                                break;
                            case ModificationType.Multiply:
                                resources.ChangeMultiplier(key, value);
                                break;
                        }
                    }
                }
            }
        }
    }
}
