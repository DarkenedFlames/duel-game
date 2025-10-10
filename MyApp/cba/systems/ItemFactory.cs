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

        public static readonly List<ItemTemplate> ItemTemplates = new()
        {
            new ItemTemplate
            {
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Rare,
                Factory = (itemEntity, player) =>
                {
                    new ItemData(itemEntity, "Sword of Light", ItemRarity.Rare, ItemType.Weapon, player);
                    new Wearable(itemEntity, EquipType.Weapon);
                    new Usable(itemEntity, 10);
                }
            },
            new ItemTemplate
            {
                Type = ItemType.Consumable,
                Rarity = ItemRarity.Common,
                Factory = (itemEntity, player) =>
                {
                    new ItemData(itemEntity, "Healing Potion", ItemRarity.Common, ItemType.Consumable, player);
                    new Usable(itemEntity, 2);
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
            // Pick type and rarity
            var itemType = PickRandom(TypeChances);
            var rarity = PickRandom(RarityChances);

            // Filter templates matching type and rarity
            var candidates = ItemTemplates
                .Where(t => t.Type == itemType && t.Rarity == rarity)
                .ToList();

            if (candidates.Count == 0)
            {
                throw new Exception($"No item template found for {itemType} with {rarity} rarity.");
            }

            // Pick a random candidate if multiple
            var chosenTemplate = candidates[rng.Next(candidates.Count)];

            // Instantiate the entity and apply components
            var itemEntity = new ItemEntity();
            chosenTemplate.Factory(itemEntity, player);

            // Add to world
            World.Instance.AddEntity(itemEntity);

            return itemEntity;
        }
    }
}
