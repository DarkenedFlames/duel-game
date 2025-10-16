namespace CBA
{
    [Flags]
    public enum ModifiesStatsTrigger
    {
        None      = 0,
        OnUse     = 1 << 0,
        OnEquip   = 1 << 1,
        OnUnequip = 1 << 2,
        OnApply   = 1 << 3, // Effect applied (entity added)
        OnRemove  = 1 << 4  // Effect removed (entity removed)
    }

    public class ModifiesStats(Entity owner, ModifiesStatsTrigger triggers) : Component(owner)
    {
        public Dictionary<string, float> StatModifiers { get; } = [];
        public Dictionary<string, int>   StatAdditions { get; } = [];
        public Dictionary<string, float> ResourceModifiers { get; } = [];
        public Dictionary<string, int>   ResourceAdditions { get; } = [];

        public ModifiesStatsTrigger Triggers { get; } = triggers;

        public event Action<Entity, Entity>? OnStatsModified;

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Item || Owner.Id.Category != EntityCategory.Effect)
                throw new InvalidOperationException($"ModifiesStats was given to an invalid category of entity: {Owner.Id}.");
            
            if (Owner.Id.Category == EntityCategory.Item && !Owner.HasComponent<Usable>() && !Owner.HasComponent<Wearable>())
                throw new InvalidOperationException($"Component Missing a Dependency: (Owner: {Owner.Id}, Component: ModifiesStats, Dependency: Usable or Wearable.");            
        }
        public override void Subscribe()
        {

            if (Owner.Id.Category == EntityCategory.Item)

                if (!Triggers.HasFlag(ModifiesStatsTrigger.OnUse) && !Triggers.HasFlag(ModifiesStatsTrigger.OnEquip))
                {
                    throw new InvalidOperationException($"ModifiesStats was given to a valid item {Owner.Id}, but no valid trigger was provided.");
                }

                if (Triggers.HasFlag(ModifiesStatsTrigger.OnUse))
                {
                    if (Owner.HasComponent<Usable>())
                    {
                        Owner.GetComponent<Usable>().OnUseSuccess += (_, target) =>
                        {
                            if (target.Id.Category != EntityCategory.Player)
                                throw new InvalidOperationException($"[{Owner.Id}] ModifiesStats was passed a non-player target: {target.Id}.");
                            Modify(target, true);
                        };
                    }
                    else
                    {
                        throw new InvalidOperationException($"ModifiesStats has the OnUse trigger but {Owner.Id} is missing Usable.");
                    }
                }

                if (Triggers.HasFlag(ModifiesStatsTrigger.OnEquip))
                {
                    if (Owner.HasComponent<Wearable>())
                    {
                        Wearable wearable = Owner.GetComponent<Wearable>();
                        Entity wearer = World.Instance.GetPlayerOf(Owner);
                        wearable.OnEquipSuccess += _ => Modify(wearer, true);
                        wearable.OnUnequipSuccess += _ => Modify(wearer, false);
                    }
                    else
                    {
                        throw new InvalidOperationException($"ModifiesStats has the OnEquip trigger but {Owner.Id} is missing Wearable.");
                    }
                }

            if (Owner.Id.Category == EntityCategory.Effect)
            {
                if (Triggers.HasFlag(ModifiesStatsTrigger.OnApply))
                {
                    Entity target = World.Instance.GetPlayerOf(Owner);
                    World.Instance.OnEntityAdded += entity => { if (entity == Owner) Modify(target, true); };
                    World.Instance.OnEntityRemoved += entity => { if (entity == Owner) Modify(target, false); };
                }
            }
        }
        private void Modify(Entity target, bool isApplying)
        {
            if (target.Id.Category != EntityCategory.Player)
                throw new InvalidOperationException($"[{Owner.Id}] ModifiesStats.Modify was passed a non-player target.");

            StatsComponent stats = target.GetComponent<StatsComponent>();
            ResourcesComponent resources = target.GetComponent<ResourcesComponent>();

            // --- Stats ---
            foreach ((string key, int value) in StatAdditions)
            {
                if (isApplying) stats.IncreaseBase(key, value);
                else            stats.DecreaseBase(key, value);
            }

            foreach ((string key, float value) in StatModifiers)
            {
                if (isApplying) stats.IncreaseModifier(key, value);
                else            stats.DecreaseModifier(key, value);
            }

            // --- Resources ---
            foreach ((string key, int value) in ResourceAdditions)
                resources.Change(key, isApplying ? value : -value);

            foreach ((string key, float value) in ResourceModifiers)
            {
                if (value == 0f) continue;
                resources.ChangeMultiplier(key, isApplying ? value : 1f / value);
            }

            OnStatsModified?.Invoke(Owner, target);
        }
    }
}
