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

        public override void Subscribe()
        {
            // --- Item logic ---
            if (Triggers.HasFlag(ModifiesStatsTrigger.OnUse))
                Owner.GetComponent<Usable>()?.OnUseSuccess += (_, target) => Modify(target, true);

            if (Triggers.HasFlag(ModifiesStatsTrigger.OnEquip) || Triggers.HasFlag(ModifiesStatsTrigger.OnUnequip))
            {
                var wearable = Owner.GetComponent<Wearable>();

                if (Triggers.HasFlag(ModifiesStatsTrigger.OnEquip))
                    wearable?.OnEquipSuccess += item =>
                        Modify(item.GetComponent<ItemData>()?.PlayerEntity, true);

                if (Triggers.HasFlag(ModifiesStatsTrigger.OnUnequip))
                    wearable?.OnUnequipSuccess += item =>
                        Modify(item.GetComponent<ItemData>()?.PlayerEntity, false);
            }

            // --- Effect logic ---
            if (Triggers.HasFlag(ModifiesStatsTrigger.OnApply))
            {
                World.Instance.OnEntityAdded += entity =>
                {
                    var player = entity.GetComponent<EffectData>()?.PlayerEntity;
                    if (entity == Owner && player != null)
                        Modify(player, true);
                };
            }

            if (Triggers.HasFlag(ModifiesStatsTrigger.OnRemove))
            {
                World.Instance.OnEntityRemoved += entity =>
                {
                    var player = entity.GetComponent<EffectData>()?.PlayerEntity;
                    if (entity == Owner && player != null)
                        Modify(player, false);
                };
            }
        }

        private void Modify(Entity? target, bool isApplying)
        {
            if (target == null) return;

            var stats = target.GetComponent<StatsComponent>();
            var resources = target.GetComponent<ResourcesComponent>();

            // --- Stats ---
            foreach (var (key, value) in StatAdditions)
            {
                if (isApplying)
                    stats?.IncreaseBase(key, value);
                else
                    stats?.DecreaseBase(key, value);
            }

            foreach (var (key, value) in StatModifiers)
            {
                if (isApplying)
                    stats?.IncreaseModifier(key, value);
                else
                    stats?.DecreaseModifier(key, value);
            }

            // --- Resources ---
            foreach (var (key, value) in ResourceAdditions)
                resources?.Change(key, isApplying ? value : -value);

            foreach (var (key, value) in ResourceModifiers)
            {
                if (value == 0f) continue;
                resources?.ChangeMultiplier(key, isApplying ? value : 1f / value);
            }

            OnStatsModified?.Invoke(Owner, target);
        }
    }
}