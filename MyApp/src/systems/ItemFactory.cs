namespace CBA
{
    public record ItemTemplate
    (
        EntityCategory Category,
        string TypeId,
        string DisplayName,
        ItemType Type,
        ItemRarity Rarity,
        List<ComponentBuilder>? Components = null
    );

    public static class ItemFactory
    {
        private static readonly Random rng = new();
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
        private static ItemTemplate GetRandomTemplate()
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
            ItemTemplate template = candidates[rng.Next(candidates.Count)];
            return template;
        }
        private static ItemTemplate GetTemplate(string typeId)
        {
            ItemTemplate template = ItemTemplates.FirstOrDefault(t =>
                 t.TypeId.Equals(typeId, StringComparison.OrdinalIgnoreCase)) ?? throw new Exception($"Item template '{typeId}' not found.");
            return template;
        }
        public static void CreateRandomItem(Entity holder) =>
            GiveItemFromTemplate(holder, GetRandomTemplate());
        public static void CreateSpecificItem(Entity holder, string typeId) =>
            GiveItemFromTemplate(holder, GetTemplate(typeId));
        public static void GiveItemFromTemplate(Entity holder, ItemTemplate template)
        {
            Entity item = new(template.Category, template.TypeId, template.DisplayName);
            if (template.Components != null)
            {
                foreach (ComponentBuilder builder in template.Components)
                {
                    Component component = builder(item, holder);
                    item.AddComponent(component);
                }
            }
            World.Instance.AddEntity(item);
        }
        public static readonly List<ItemTemplate> ItemTemplates =
        [
            // ==========================================================
            // Consumables
            // ==========================================================
            #region
            new(
                EntityCategory.Item,
                "ruby_red_remedy",
                "Ruby Red Remedy",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new DealsHealing(e, 25)
                ]
            ),
            new(
                EntityCategory.Item,
                "celestial_cyan_cider",
                "Celestial Cyan Cider",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 0),
                    (e, holder) => new DealsStimming(e, 25)
                ]
            ),
            new(
                EntityCategory.Item,
                "yummy_yellow_yarb",
                "Yummy Yellow Yarb",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new() {["MaximumHealth"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "baby_blue_brew",
                "Baby Blue Brew",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new(){["MaximumStamina"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "pale_purple_potion",
                "Pale Purple Potion",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new(){["Healing"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "glowing_green_grog",
                "Glowing Green Grog",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new(){["Stimming"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "oozing_orange_oil",
                "Oozing Orange Oil",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new(){["Armor"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "alluring_azure_amalgam",
                "Alluring Azure Amalgam",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new(){["Attack"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "tall_teal_tincture",
                "Tall Teal Tincture",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new(){["Shield"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "refreshing_rose_rum",
                "Refreshing Rose Rum",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new(){["Precision"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "vivid_violet_vase",
                "Vivid Violet Vase",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new(){["Dodge"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "lovely_lime_liquid",
                "Lovely Lime Liquid",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new(){["Critical"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "null_nib",
                "Null Nib",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new(){["Luck"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "void_vial",
                "Void Vial",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new(){["Attack"]  = 25.0f}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "chromatic_concoction",
                "Chromatic Concoction",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUse)] = new()
                            {
                                ["MaximumHealth"]  = 5.0f,
                                ["MaximumStamina"] = 5.0f,
                                ["Healing"]        = 5.0f,
                                ["Stimming"]       = 5.0f,
                                ["Armor"]          = 5.0f,
                                ["Shield"]         = 5.0f,
                                ["Attack"]         = 5.0f,
                                ["Precision"]      = 5.0f,
                                ["Dodge"]          = 5.0f,
                                ["Critical"]       = 5.0f,
                                ["Luck"]           = 5.0f,
                                ["Steal"]          = 5.0f,
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "electric_enhancement",
                "Electric Enhancement",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesEffects(e,
                    effectsByTrigger: new()
                        {
                            [(EffectAction.Remove, TargetType.Self, Trigger.OnUse)] =
                            [
                                "inferno", "crushed", "frozen", "blinded"
                            ]
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "magnetic_modification",
                "Magnetic Modification",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesEffects(e,
                    effectsByTrigger: new()
                        {
                            [(EffectAction.Apply, TargetType.Self, Trigger.OnUse)] =
                            [
                                "noons_visibility"
                            ]
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "metamedicine",
                "Metamedicine",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new ModifiesEffects(e,
                    effectsByTrigger: new()
                        {
                            [(EffectAction.Remove, TargetType.Self, Trigger.OnUse)] =
                            [
                                "inferno", "crushed", "frozen", "blinded"
                            ],
                            [(EffectAction.Apply, TargetType.Self, Trigger.OnUse)] =
                            [
                                "noons_visibility"
                            ]
                        }
                    )
                ]
            ),
            #endregion
            // ==========================================================
            // Weapons
            // ==========================================================
            #region
            new(
                EntityCategory.Item,
                "brandish",
                "Brandish",
                ItemType.Weapon,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Weapon),
                    (e, holder) => new Wearable(e, EquipType.Weapon),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new Hits(e, true),
                    (e, holder) => new DealsDamage(e, 10, DamageType.Physical, true),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Target, Trigger.OnUse), ["inferno"]}
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "crescent",
                "Crescent",
                ItemType.Weapon,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Weapon),
                    (e, holder) => new Wearable(e, EquipType.Weapon),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new Hits(e, true),
                    (e, holder) => new DealsDamage(e, 10, DamageType.Physical, true),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnHit), ["moonlight"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnUnequip), ["moonlight"] },
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnTurnStartWhileEquipped), ["moonlight"] },
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnEquip), ["moonlight"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "terrain",
                "Terrain",
                ItemType.Weapon,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Weapon),
                    (e, holder) => new Wearable(e, EquipType.Weapon),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new Hits(e, true),
                    (e, holder) => new DealsDamage(e, 10, DamageType.Physical, true),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Target, Trigger.OnHit), ["crushed"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "eclipse",
                "Eclipse",
                ItemType.Weapon,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Weapon),
                    (e, holder) => new Wearable(e, EquipType.Weapon),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new Hits(e, true),
                    (e, holder) => new DealsDamage(e, 10, DamageType.Physical, true),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Target, Trigger.OnHit), ["blinded"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "solstice",
                "Solstice",
                ItemType.Weapon,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Weapon),
                    (e, holder) => new Wearable(e, EquipType.Weapon),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new Hits(e, true),
                    (e, holder) => new DealsDamage(e, 10, DamageType.Physical, true),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnTurnStartWhileEquipped), ["noons_visibility"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "hibernal",
                "Hibernal",
                ItemType.Weapon,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Weapon),
                    (e, holder) => new Wearable(e, EquipType.Weapon),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new Hits(e, true),
                    (e, holder) => new DealsDamage(e, 10, DamageType.Physical, true),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Target, Trigger.OnHit), ["frozen"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "corona",
                "Corona",
                ItemType.Weapon,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Weapon),
                    (e, holder) => new Wearable(e, EquipType.Weapon),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new Hits(e, true),
                    (e, holder) => new DealsDamage(e, 10, DamageType.Physical, true),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Target, Trigger.OnHit), ["sunburn"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "aurora",
                "Aurora",
                ItemType.Weapon,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Weapon),
                    (e, holder) => new Wearable(e, EquipType.Weapon),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new Hits(e, true),
                    (e, holder) => new DealsDamage(e, 5, DamageType.True, true)
                ]
            ),
            new(
                EntityCategory.Item,
                "harvest",
                "Harvest",
                ItemType.Weapon,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Weapon),
                    (e, holder) => new Wearable(e, EquipType.Weapon),
                    (e, holder) => new Usable(e, 25),
                    (e, holder) => new Hits(e, true),
                    (e, holder) => new DealsDamage(e, 10, DamageType.True, true),
                    (e, holder) => new DealsHealing(e, 5)
                ]
            ),
            #endregion
            // ==========================================================
            // Armor
            // ==========================================================
            #region
            // Oculus Set
            #region
            new(
                EntityCategory.Item,
                "oculus_helmet",
                "Oculus Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "oculus"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["third_eye"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["third_eye"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "oculus_chestplate",
                "Oculus Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "oculus"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["third_eye"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["third_eye"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "oculus_leggings",
                "Oculus Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "oculus"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["third_eye"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["third_eye"] }
                        }
                    )
                ]
            ),
            #endregion
            // Sonic Set
            #region
            new(
                EntityCategory.Item,
                "sonic_helmet",
                "Sonic Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "sonic"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["vibrata"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["vibrata"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "sonic_chestplate",
                "Sonic Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "sonic"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["vibrata"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["vibrata"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "sonic_leggings",
                "Sonic Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "sonic"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["vibrata"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["vibrata"] }
                        }
                    )
                ]
            ),
            #endregion
            // Olfactory Set
            #region
            new(
                EntityCategory.Item,
                "olfactory_helmet",
                "Olfactory Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "olfactory"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["pungence"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["pungence"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "olfactory_chestplate",
                "Olfactory Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "olfactory"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["pungence"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["pungence"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "olfactory_leggings",
                "Olfactory Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "olfactory"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["pungence"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["pungence"] }
                        }
                    )
                ]
            ),
            #endregion
            // Tact Set
            #region
            new(
                EntityCategory.Item,
                "tact_helmet",
                "Tact Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "tact"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["dexterity"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["dexterity"] }
                        }
                    )
                ]
            ),

            new(
                EntityCategory.Item,
                "tact_chestplate",
                "Tact Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "tact"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["dexterity"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["dexterity"] }
                        }
                    )
                ]
            ),

            new(
                EntityCategory.Item,
                "tact_leggings",
                "Tact Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "tact"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["dexterity"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["dexterity"] }
                        }
                    )
                ]
            ),
            #endregion
            // Relish Set
            #region
            new(
                EntityCategory.Item,
                "relish_helmet",
                "Relish Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "relish"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["delight"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["delight"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "relish_chestplate",
                "Relish Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "relish"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["delight"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["delight"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "relish_leggings",
                "Relish Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "relish"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["delight"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["delight"] }
                        }
                    )
                ]
            ),
            #endregion
            // Vanity Set
            #region
            new(
                EntityCategory.Item,
                "vanity_helmet",
                "Vanity Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "vanity"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["reflection"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["relection"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "vanity_chestplate",
                "Vanity Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "vanity"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["reflection"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["relection"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "vanity_leggings",
                "Vanity Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "vanity"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["reflection"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["relection"] }
                        }
                    )
                ]
            ),
            #endregion
            // Avarice Set
            #region
            new(
                EntityCategory.Item,
                "avarice_helmet",
                "Avarice Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "avarice"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["pleonexia"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["pleonexia"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "avarice_chestplate",
                "Avarice Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "avarice"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["pleonexia"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["pleonexia"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "avarice_leggings",
                "Avarice Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "avarice"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["pleonexia"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["pleonexia"] }
                        }
                    )
                ]
            ),
            #endregion
            // Temper Set
            #region
            new(
                EntityCategory.Item,
                "temper_helmet",
                "Temper Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "temper"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["rage"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["rage"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "temper_chestplate",
                "Temper Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "temper"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["rage"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["rage"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "temper_leggings",
                "Temper Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "temper"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["rage"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["rage"] }
                        }
                    )
                ]
            ),
            #endregion
            // Covet Set
            #region
            new(
                EntityCategory.Item,
                "covet_helmet",
                "Covet Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "covet"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["wanting"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["wanting"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "covet_chestplate",
                "Covet Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "covet"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["wanting"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["wanting"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "covet_leggings",
                "Covet Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "covet"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["wanting"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["wanting"] }
                        }
                    )
                ]
            ),
            #endregion
            // Lax Set
            #region
            new(
                EntityCategory.Item,
                "lax_helmet",
                "Lax Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "lax"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["awakening"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["awakening"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "lax_chestplate",
                "Lax Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "lax"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["awakening"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["awakening"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "lax_leggings",
                "Lax Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "lax"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["awakening"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["awakening"] }
                        }
                    )
                ]
            ),
            #endregion
            // Voracity Set
            #region
            new(
                EntityCategory.Item,
                "voracity_helmet",
                "Voracity Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "voracity"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["devouring"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["devouring"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "voracity_chestplate",
                "Voracity Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "voracity"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["devouring"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["devouring"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "voracity_leggings",
                "Voracity Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "voracity"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["devouring"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["devouring"] }
                        }
                    )
                ]
            ),
            #endregion
            // Lecher Set
            #region
            new(
                EntityCategory.Item,
                "lecher_helmet",
                "Lecher Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Helmet),
                    (e, holder) => new CompletesItemSet(e, "lecher"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["cheating"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["cheating"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "lecher_chestplate",
                "Lecher Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Chestplate),
                    (e, holder) => new CompletesItemSet(e, "lecher"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["cheating"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["cheating"] }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "lecher_leggings",
                "Lecher Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Armor),
                    (e, holder) => new Wearable(e, EquipType.Leggings),
                    (e, holder) => new CompletesItemSet(e, "lecher"),
                    (e, holder) => new ModifiesStats(e,
                        statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnEquip)] = new()
                            {
                                ["Armor"]  = 50f,
                                ["Shield"] = 50f,
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnUnequip)] = new()
                            {
                                ["Armor"]  = -50f,
                                ["Shield"] = -50f,
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        effectsByTrigger: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["cheating"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["cheating"] }
                        }
                    )
                ]
            ),
            #endregion
            #endregion
            // Add more templates as needed
        ];
    }
}
