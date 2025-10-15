using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CBA
{
    public record ItemTemplate
    (
        EntityCategory Category,
        string TypeId,
        string DisplayName,
        ItemType Type,
        ItemRarity Rarity,
        int? StaminaCost = null,
        EquipType? EquipType = null,
        ModifiesStatsTrigger? StatsTrigger = null,
        Dictionary<string, int>? StatAdditions = null,
        Dictionary<string, int>? ResourceAdditions = null,
        Dictionary<string, float>? StatModifiers = null,
        Dictionary<string, float>? ResourceModifiers = null,
        Dictionary<EffectTrigger, List<string>>? EffectsByTrigger = null,
        int Damage = 0,
        DamageType DamageType = DamageType.Physical,
        bool CanCrit = true,
        bool CanDodge = true
    );

    public static class ItemFactory
    {
        private static readonly Random rng = new();

        // Type and rarity chances
        private static readonly Dictionary<ItemType, double> TypeChances = new()
        {
            { ItemType.Consumable, 0.25 },
            { ItemType.Weapon, 0.25 },
            { ItemType.Armor, 0.25 },
            { ItemType.Accessory, 0.25 }
        };

        private static readonly Dictionary<ItemRarity, double> RarityChances = new()
        {
            { ItemRarity.Common, 0.90 },
            { ItemRarity.Uncommon, 0.09 },
            { ItemRarity.Rare, 0.009 },
            { ItemRarity.Mythical, 0.001 }
        };


        public static readonly List<ItemTemplate> ItemTemplates =
        [
            // ==========================================================
            // Consumables
            // ==========================================================
            //     ======================================================
            //     Common
            //     ======================================================

            new(
                EntityCategory.Item,
                "ruby_red_remedy",
                "Ruby Red Remedy",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                ResourceAdditions: new() { { "Health", 25 } }
            ),
            new(
                EntityCategory.Item,
                "baby_blue_brew",
                "Baby Blue Brew",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 0,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                ResourceAdditions: new() { { "Stamina", 25 } }
            ),
            new(
                EntityCategory.Item,
                "oozing_orange_oil",
                "Oozing Orange Oil",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                StatAdditions: new() { { "MaximumHealth", 5 } }
            ),
            new(
                EntityCategory.Item,
                "glowing_green_grog",
                "Glowing Green Grog",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                StatAdditions: new() { { "MaximumStamina", 5 } }
            ),
            new(
                EntityCategory.Item,
                "yielding_yellow_yarb",
                "Yielding Yellow Yarb",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                StatAdditions: new() { { "Armor", 5 } }
            ),
            new(
                EntityCategory.Item,
                "pale_purple_potion",
                "Pale Purple Potion",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                StatAdditions: new() { { "Shield", 5 } }
            ),
            new(
                EntityCategory.Item,
                "pupil_porridge",
                "Pupil Porridge",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                StatAdditions: new() { { "Critical", 5 } }
            ),
            new(
                EntityCategory.Item,
                "cartilage_chowder",
                "Cartilage Chowder",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                StatAdditions: new() { { "Dodge", 5 } }
            ),
            new(
                EntityCategory.Item,
                "magpie_morsel",
                "Magpie Morsel",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                StatAdditions: new() { { "Luck", 5 } }
            ),
            new(
                EntityCategory.Item,
                "vulture_vittles",
                "Vulture Vittles",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                StatAdditions: new() { { "Peer", 5 } }
            ),

            //     ======================================================
            //     Uncommon
            //     ======================================================

            new(
                EntityCategory.Item,
                "auburn_amalgam",
                "Auburn Amalgam",
                ItemType.Consumable,
                ItemRarity.Uncommon,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                StatModifiers: new() { { "MaximumHealth", 1.25f } },
                ResourceModifiers: new() { {"Health", 1.25f} }
            ),
            new(
                EntityCategory.Item,
                "teal_tincture",
                "Teal Tincture",
                ItemType.Consumable,
                ItemRarity.Uncommon,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                StatModifiers: new() { { "MaximumStamina", 1.25f } },
                ResourceModifiers: new() { {"Stamina", 1.25f} }
            ),
            new(
                EntityCategory.Item,
                "earthy_elixir",
                "Earthy Elixir",
                ItemType.Consumable,
                ItemRarity.Uncommon,
                StaminaCost: 15,
                StatsTrigger: ModifiesStatsTrigger.OnUse,
                StatModifiers: new() { { "Armor", 1.25f }, {"Shield", 1.25f} }
            ),

            // ==========================================================
            // Weapons
            // ==========================================================
            //     ======================================================
            //     Common
            //     ======================================================

            new(
                EntityCategory.Item,
                "brandish",
                "Brandish",
                ItemType.Weapon,
                ItemRarity.Common,
                StaminaCost: 25,
                EquipType: EquipType.Weapon,
                Damage: 10,
                DamageType: DamageType.Physical,
                CanCrit: true,
                CanDodge: true,
                EffectsByTrigger: new() {{EffectTrigger.OnUse, ["inferno"]}}
            ),
            // Add more templates as needed
        ];

        public static void AddComponents(Entity item, Entity player, ItemTemplate template)
        {
            // Change Item Data to not take display name
            new ItemData(item, template.DisplayName, template.Rarity, template.Type, player);

            if (template.Type == ItemType.Consumable) // Only Consumables get Consumable
                new Consumable(item);

            if ((template.Type == ItemType.Consumable || template.Type == ItemType.Weapon) &&
                template.StaminaCost is int staminaCost)
            {
                new Usable(item, staminaCost);
            }    // Consumables or Weapons get Usable if their StaminaCost is defined.
            
            if (template.Type == ItemType.Weapon)
            {
                new DealsDamage(item, template.Damage, template.DamageType, template.CanCrit, template.CanDodge);
            }
            
            if (template.EquipType is EquipType equipType) // Non-null EquipType gets Wearable
                new Wearable(item, equipType);

            if
            (
                template.StatAdditions != null ||
                template.StatModifiers != null ||
                template.ResourceAdditions != null ||
                template.ResourceModifiers != null
            )
            {
                if (template.StatsTrigger is ModifiesStatsTrigger statsTrigger)
                {
                    var modifiesStats = new ModifiesStats(item, statsTrigger);

                    if (template.StatAdditions != null)
                    {
                        foreach (var (k, v) in template.StatAdditions)
                            modifiesStats.StatAdditions[k] = v;
                    }
                    if (template.StatModifiers != null)
                    {
                        foreach (var (k, v) in template.StatModifiers)
                            modifiesStats.StatModifiers[k] = v;
                    }
                    if (template.ResourceAdditions != null)
                    {
                        foreach (var (k, v) in template.ResourceAdditions)
                            modifiesStats.ResourceAdditions[k] = v;
                    }
                    if (template.ResourceModifiers != null)
                    {
                        foreach (var (k, v) in template.ResourceModifiers)
                            modifiesStats.ResourceModifiers[k] = v;
                    }
                }

            }
        
            if (template.EffectsByTrigger != null)
            {
                var modifiesEffects = new ModifiesEffects(item);
                foreach(var (k, v) in template.EffectsByTrigger)
                {
                    modifiesEffects.TriggeredEffects[k] = v;
                }
            }
        }

        // Generic method to pick a random key from a dictionary based on chance
        private static T PickRandom<T>(Dictionary<T, double> chances) where T : struct, Enum
        {
            double total = chances.Values.Sum();
            double roll = rng.NextDouble() * total;
            double cumulative = 0;

            foreach (KeyValuePair<T, double> kvp in chances)
            {
                cumulative += kvp.Value;
                if (roll <= cumulative)
                    return kvp.Key;
            }

            // Fallback (should rarely happen)
            return chances.Keys.First();
        }

        public static Entity CreateRandomItem(Entity player)
        {
            // --- Step 1: Pick type and rarity based on weights ---
            ItemType itemType = PickRandom(TypeChances);
            ItemRarity rarity = PickRandom(RarityChances);

            // --- Step 2: Find exact matches ---
            List<ItemTemplate>? candidates = [.. ItemTemplates.Where(t => t.Type == itemType && t.Rarity == rarity)];

            // --- Step 3: Fallback: same type, lower rarities ---
            if (candidates.Count == 0)
            {
                IOrderedEnumerable<ItemRarity>? lowerRarities = RarityChances.Keys
                    .Where(r => r < rarity)
                    .OrderByDescending(r => r); // closest lower rarity first

                foreach (ItemRarity r in lowerRarities)
                {
                    candidates = [.. ItemTemplates.Where(t => t.Type == itemType && t.Rarity == r)];
                    if (candidates.Count > 0) break;
                }
            }

            // --- Step 4: Fallback: any type, closest rarity ---
            if (candidates.Count == 0)
            {
                IOrderedEnumerable<ItemRarity>? raritiesByDistance = RarityChances.Keys
                    .OrderBy(r => Math.Abs((int)r - (int)rarity));

                foreach (ItemRarity r in raritiesByDistance)
                {
                    candidates = [.. ItemTemplates.Where(t => t.Rarity == r)];
                    if (candidates.Count > 0) break;
                }
            }

            // --- Step 5: Ultimate fallback: pick anything ---
            if (candidates.Count == 0)
                candidates = [.. ItemTemplates];

            // --- Step 6: Pick one randomly from remaining candidates ---
            ItemTemplate temp = candidates[rng.Next(candidates.Count)];

            // --- Step 7: Instantiate the entity and apply components ---
            Entity itemEntity = new(temp.Category, temp.TypeId, temp.DisplayName);
            AddComponents(itemEntity, player, temp);
            World.Instance.AddEntity(itemEntity);

            return itemEntity;
        }

    }
}
