using System;
using System.Collections.Generic;
using System.Linq;

namespace CBA
{
    // Concrete entity class for items
    public class ItemEntity : Entity
    {
        // No extra members; just concrete so we can instantiate
    }

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

        // Templates for all possible items
        public class ItemTemplate
        {
            public ItemType Type { get; init; }
            public ItemRarity Rarity { get; init; }
            public Action<Entity, Entity> Factory { get; init; } = null!;
        }

        // Soon change how components subscribe so that order won't matter.
        public static readonly List<ItemTemplate> ItemTemplates = new()
        {
            // ==========================================================
            // Consumables
            // ==========================================================
            //     ======================================================
            //     Common
            //     ======================================================

            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Ruby Red Remedy", ItemRarity.Common, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.ResourceAdditions["Health"] = 25;
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Baby Blue Brew", ItemRarity.Common, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.ResourceAdditions["Stamina"] = 25;
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Oozing Orange Oil", ItemRarity.Common, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.StatAdditions["MaximumHealth"] = 5;
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Glowing Green Grog", ItemRarity.Common, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.StatAdditions["MaximumStamina"] = 5;
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Yielding Yellow Yarb", ItemRarity.Common, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.StatAdditions["Armor"] = 5;
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Pale Purple Potion", ItemRarity.Common, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.StatAdditions["Shield"] = 5;
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Pupil Porridge", ItemRarity.Common, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.StatAdditions["Critical"] = 5;
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Cartilage Chowder", ItemRarity.Common, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.StatAdditions["Dodge"] = 5;
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Magpie Morsel", ItemRarity.Common, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.StatAdditions["Luck"] = 5;
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Vulture Vittles", ItemRarity.Common, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.StatAdditions["Peer"] = 5;
                }
            },

            //     ======================================================
            //     Uncommon
            //     ======================================================

            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Uncommon,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Auburn Amalgam", ItemRarity.Uncommon, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.StatModifiers["MaximumHealth"] = 1.25f;
                    modifiesStats.ResourceModifiers["Health"] = 1.25f;
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Uncommon,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Teal Tincture", ItemRarity.Uncommon, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.StatModifiers["MaximumStamina"] = 1.25f;
                    modifiesStats.ResourceModifiers["Stamina"] = 1.25f;
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Uncommon,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Earthy Elixir", ItemRarity.Uncommon, ItemType.Consumable, player);
                    new Usable(itemEntity, 15);
                    new Consumable(itemEntity);
                    var modifiesStats = new ModifiesStats(itemEntity, ModifiesStatsTrigger.OnUse);
                    modifiesStats.StatModifiers["Armor"] = 1.25f;
                    modifiesStats.StatModifiers["Shield"] = 1.25f;
                }
            },

            // ==========================================================
            // Weapons
            // ==========================================================
            //     ======================================================
            //     Common
            //     ======================================================

            new ItemTemplate
            {
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    // Order matters, some components depend on others.
                    new ItemData(itemEntity, "Brandish", ItemRarity.Common, ItemType.Weapon, player);
                    new Usable(itemEntity, 25);
                    new DealsDamage(itemEntity, damage: 10, DamageType.Physical, canCrit: true, canDodge: true);
                    var modifiesEffects = new ModifiesEffects(itemEntity);
                    modifiesEffects.TriggeredEffects[EffectTrigger.OnUse] = [
                        new EffectInfo(){ EffectName = "Inferno", TargetType = EffectTargetType.Target}
                    ];
                }
            },
            // Add more templates as needed
        };

        // Generic method to pick a random key from a dictionary based on chance
        private static T PickRandom<T>(Dictionary<T, double> chances) where T : struct, Enum
        {
            double total = chances.Values.Sum();
            double roll = rng.NextDouble() * total;
            double cumulative = 0;

            foreach (var kvp in chances)
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
            var itemType = PickRandom(TypeChances);
            var rarity = PickRandom(RarityChances);

            // --- Step 2: Find exact matches ---
            var candidates = ItemTemplates
                .Where(t => t.Type == itemType && t.Rarity == rarity)
                .ToList();

            // --- Step 3: Fallback: same type, lower rarities ---
            if (candidates.Count == 0)
            {
                var lowerRarities = RarityChances.Keys
                    .Where(r => r < rarity)
                    .OrderByDescending(r => r); // closest lower rarity first

                foreach (var r in lowerRarities)
                {
                    candidates = ItemTemplates
                        .Where(t => t.Type == itemType && t.Rarity == r)
                        .ToList();
                    if (candidates.Count > 0) break;
                }
            }

            // --- Step 4: Fallback: any type, closest rarity ---
            if (candidates.Count == 0)
            {
                var raritiesByDistance = RarityChances.Keys
                    .OrderBy(r => Math.Abs((int)r - (int)rarity));

                foreach (var r in raritiesByDistance)
                {
                    candidates = [.. ItemTemplates.Where(t => t.Rarity == r)];
                    if (candidates.Count > 0) break;
                }
            }

            // --- Step 5: Ultimate fallback: pick anything ---
            if (candidates.Count == 0)
                candidates = [.. ItemTemplates];

            // --- Step 6: Pick one randomly from remaining candidates ---
            var chosenTemplate = candidates[rng.Next(candidates.Count)];

            // --- Step 7: Instantiate the entity and apply components ---
            var itemEntity = new ItemEntity();
            chosenTemplate.Factory(itemEntity, player);
            itemEntity.SubscribeAll();

            // --- Step 8: Add to world ---
            // automatic

            return itemEntity;
        }

    }
}
