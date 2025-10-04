using System;
using System.Collections.Generic;

namespace MyApp
{
    public static class ItemFactory
    {
        // Each factory now takes a Player and returns an Item
        public static List<Func<Player, Item>> AllItems = new()
        {
            // Weapons
            player => new Brandish(player),
            player => new Crescent(player),
            // Consumables
            player => new RubyRedRemedy(player),
            player => new BabyBlueBrew(player),
            player => new OozingOrangeOil(player),
            player => new YieldingYellowYarb(player),
            player => new PalePurplePotion(player),
            player => new PupilPorridge(player),
            player => new CartilageChowder(player),
            player => new MagpieMorsel(player),
            player => new VultureVittles(player),
            // Add more items here...
        };

        /// <summary>
        /// Picks a random item (weighted by rarity/type) and immediately gives it to the player.
        /// </summary>
        public static void GiveRandomItem(Player player)
        {
            // Step 1: Choose Rarity
            var rarity = WeightedRandom.Choose(new Dictionary<ItemRarity, double>
            {
                { ItemRarity.Common, 90 },
                { ItemRarity.Uncommon, 9 },
                { ItemRarity.Rare, 0.9 },
                { ItemRarity.Mythical, 0.1 }
            });

            // Step 2: Choose Type category (25% each group)
            var category = WeightedRandom.Choose(new Dictionary<string, double>
            {
                { "Consumable", 25 },
                { "Weapon", 25 },
                { "Armor", 25 },
                { "Accessory", 25 }
            });

            // Step 3: Pick candidates that match rarity & type
            var candidates = new List<Func<Player, Item>>();
            foreach (var factory in AllItems)
            {
                var tempItem = factory(player);
                bool matchesRarity = tempItem.Rarity == rarity;
                bool matchesCategory =
                    (category == "Armor" && (tempItem.Type == ItemType.Helmet ||
                                             tempItem.Type == ItemType.Chestplate ||
                                             tempItem.Type == ItemType.Leggings)) ||
                    (category == "Consumable" && tempItem.Type == ItemType.Consumable) ||
                    (category == "Weapon" && tempItem.Type == ItemType.Weapon) ||
                    (category == "Accessory" && tempItem.Type == ItemType.Accessory);

                if (matchesRarity && matchesCategory)
                    candidates.Add(factory);
            }

            // Fallback to any of the correct rarity
            if (candidates.Count == 0)
            {
                foreach (var factory in AllItems)
                {
                    var tempItem = factory(player);
                    if (tempItem.Rarity == rarity)
                        candidates.Add(factory);
                }
            }

            // Fallback to all items
            if (candidates.Count == 0)
                candidates.AddRange(AllItems);

            // Step 4: Instantiate a random candidate and give it to the player
            var chosenFactory = candidates[new Random().Next(candidates.Count)];
            var item = chosenFactory(player);
            player.AddItem(item); // Owner is already set in constructor
        }
    }
}
