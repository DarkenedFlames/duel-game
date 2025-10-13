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
            {
                Usable usable = Helper.ThisIsNotNull(
                    Owner.GetComponent<Usable>(),
                    "ModifiesStats requires Usable for OnUse trigger."
                );

                usable.OnUseSuccess += (_, target) =>
                    Modify(Helper.ThisIsNotNull(target, "Target cannot be null on use."), true);
            }

            if (Triggers.HasFlag(ModifiesStatsTrigger.OnEquip) || 
                Triggers.HasFlag(ModifiesStatsTrigger.OnUnequip))
            {
                Wearable wearable = Helper.ThisIsNotNull(
                    Owner.GetComponent<Wearable>(),
                    "ModifiesStats requires Wearable for OnEquip/OnUnequip trigger."
                );

                if (Triggers.HasFlag(ModifiesStatsTrigger.OnEquip))
                {
                    wearable.OnEquipSuccess += item =>
                    {
                        ItemData itemData = Helper.ThisIsNotNull(
                            item.GetComponent<ItemData>(),
                            "ItemData missing for OnEquip."
                        );
                        Entity wearer = Helper.ThisIsNotNull(
                            itemData.PlayerEntity,
                            "PlayerEntity missing for OnEquip."
                        );
                        Modify(wearer, true);
                    };
                }

                if (Triggers.HasFlag(ModifiesStatsTrigger.OnUnequip))
                {
                    wearable.OnUnequipSuccess += item =>
                    {
                        ItemData itemData = Helper.ThisIsNotNull(
                            item.GetComponent<ItemData>(),
                            "ItemData missing for OnUnequip."
                        );
                        Entity wearer = Helper.ThisIsNotNull(
                            itemData.PlayerEntity,
                            "PlayerEntity missing for OnUnequip."
                        );
                        Modify(wearer, false);
                    };
                }
            }

            // --- Effect logic ---
            if (Triggers.HasFlag(ModifiesStatsTrigger.OnApply))
            {
                World.Instance.OnEntityAdded += entity =>
                {
                    EffectData? effectData = entity.GetComponent<EffectData>();
                    if (entity == Owner && effectData?.PlayerEntity is Entity player)
                        Modify(player, true);
                };
            }

            if (Triggers.HasFlag(ModifiesStatsTrigger.OnRemove))
            {
                World.Instance.OnEntityRemoved += entity =>
                {
                    EffectData? effectData = entity.GetComponent<EffectData>();
                    if (entity == Owner && effectData?.PlayerEntity is Entity player)
                        Modify(player, false);
                };
            }
        }

        private void Modify(Entity target, bool isApplying)
        {
            StatsComponent stats = Helper.ThisIsNotNull(
                target.GetComponent<StatsComponent>(),
                $"StatsComponent missing on {target}."
            );

            ResourcesComponent resources = Helper.ThisIsNotNull(
                target.GetComponent<ResourcesComponent>(),
                $"ResourcesComponent missing on {target}."
            );

            // --- Stats ---
            foreach ((string key, int value) in StatAdditions)
            {
                if (isApplying)
                    stats.IncreaseBase(key, value);
                else
                    stats.DecreaseBase(key, value);
            }

            foreach ((string key, float value) in StatModifiers)
            {
                if (isApplying)
                    stats.IncreaseModifier(key, value);
                else
                    stats.DecreaseModifier(key, value);
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
