namespace CBA
{
    [Flags]
    public enum ModifiesStatsTrigger
    {
        None        = 0,
        OnUse       = 1 << 0,
        OnEquip     = 1 << 1,
        OnUnequip   = 1 << 2,
        OnAdded     = 1 << 3, // Effect applied (entity added)
        OnRemoved   = 1 << 4, // Effect removed (entity removed)
        OnHit       = 1 << 5,
        OnCritical  = 1 << 6,
        OnDamageDealt = 1 << 7
    }

    public enum ModificationType
    {
        Add,
        Multiply
    }

    public class ModifiesStats(
        Entity owner,
        Dictionary<(ModifiesStatsTrigger, ModificationType), Dictionary<string, float>>? StatsByTrigger = null,
        Dictionary<(ModifiesStatsTrigger, ModificationType), Dictionary<string, float>>? ResourcesByTrigger = null) : Component(owner)
    {
        // <(Trigger, ModificationType), Dictionary<StatName, Value>>
        public Dictionary<(ModifiesStatsTrigger Trigger, ModificationType Type), Dictionary<string, float>>? StatChanges { get; } = StatsByTrigger;
        public Dictionary<(ModifiesStatsTrigger Trigger, ModificationType Type), Dictionary<string, float>>? ResourceChanges { get; } = ResourcesByTrigger;
        
        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Item && Owner.Id.Category != EntityCategory.Effect)
                throw new InvalidOperationException($"{Owner.Id} was given an invalid Component: ModifiesStats.");

            // Fail loud if triggers are declared that don't match owner type.
            bool requiresUsable = HasTrigger(ModifiesStatsTrigger.OnUse);
            bool requiresWearable = HasTrigger(ModifiesStatsTrigger.OnEquip) || HasTrigger(ModifiesStatsTrigger.OnUnequip);

            if (requiresUsable && !Owner.HasComponent<Usable>())
                throw new InvalidOperationException($"{Owner.Id} declares OnUse but lacks Usable.");

            if (requiresWearable && !Owner.HasComponent<Wearable>())
                throw new InvalidOperationException($"{Owner.Id} declares OnEquip/OnUnequip but lacks Wearable.");
        }

        public override void Subscribe()
        {
            // For each declared trigger, subscribe appropriately.
            foreach (ModifiesStatsTrigger trigger in Enum.GetValues<ModifiesStatsTrigger>())
            {
                if (trigger == ModifiesStatsTrigger.None) continue;
                if (!HasTrigger(trigger)) continue;

                switch (trigger)
                {
                    case ModifiesStatsTrigger.OnEquip:
                    case ModifiesStatsTrigger.OnUnequip:
                    {
                        var wearable = Owner.GetComponent<Wearable>();
                        var wearer = World.Instance.GetPlayerOf(Owner);
                        if (trigger == ModifiesStatsTrigger.OnEquip)
                            wearable.OnEquipSuccess += _ => Modify(wearer, ModifiesStatsTrigger.OnEquip);
                        else
                            wearable.OnUnequipSuccess += _ => Modify(wearer, ModifiesStatsTrigger.OnUnequip);
                        break;
                    }
                    case ModifiesStatsTrigger.OnAdded:
                    case ModifiesStatsTrigger.OnRemoved:
                    {
                        var target = World.Instance.GetPlayerOf(Owner);

                        if (trigger == ModifiesStatsTrigger.OnAdded)
                            World.Instance.OnEntityAdded += e => { if (e == Owner) Modify(target, trigger); };
                        else
                            World.Instance.OnEntityRemoved += e => { if (e == Owner) Modify(target, trigger); };
                        
                        break;
                    }
                    case ModifiesStatsTrigger.OnUse:
                        Owner.GetComponent<Usable>().OnUseSuccess += (_, target) => Modify(target, trigger);
                        break;
                    case ModifiesStatsTrigger.OnHit:
                        Owner.GetComponent<Hits>().OnHit += (_, target) => Modify(target, trigger);
                        break;
                    case ModifiesStatsTrigger.OnCritical:
                        Owner.GetComponent<DealsDamage>().OnCritical += (_, target) => Modify(target, trigger);
                        break;
                    case ModifiesStatsTrigger.OnDamageDealt:
                        Owner.GetComponent<DealsDamage>().OnDamageDealt += (_, target, _) => Modify(target, trigger);
                        break;
                }
            }
        }

        private bool HasTrigger(ModifiesStatsTrigger trigger)
        {
            return (StatChanges?.Keys.Any(k => k.Trigger == trigger) ?? false) ||
                   (ResourceChanges?.Keys.Any(k => k.Trigger == trigger) ?? false);
        }

        private void Modify(Entity target, ModifiesStatsTrigger trigger)
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
                            case ModificationType.Add: stats.IncreaseBase(key, (int)value); break;
                            case ModificationType.Multiply: stats.IncreaseModifier(key, value); break;
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
                            case ModificationType.Add: resources.Change(key, (int)value); break;
                            case ModificationType.Multiply: resources.ChangeMultiplier(key, value); break;
                        }
                    }
                }
            }
        }
    }
}
