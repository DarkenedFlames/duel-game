namespace CBA
{
    public record ItemTemplate
    (
        EntityCategory Category,
        string TypeId,
        string DisplayName,
        ItemType Type,
        ItemRarity Rarity,
        string? SetTag = null,
        int? StaminaCost = null,
        EquipType? EquipType = null,
        Dictionary<(Trigger, ModificationType), Dictionary<string, float>>? StatsByTrigger = null,
        Dictionary<(Trigger, ModificationType), Dictionary<string, float>>? ResourcesByTrigger = null,
        Dictionary<(EffectAction, TargetType, Trigger), List<string>>? EffectsByTrigger = null,
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
                ResourcesByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Add)] = new()
                    {
                        ["Health"]  = 25.0f
                    }
                }
            ),
            new(
                EntityCategory.Item,
                "baby_blue_brew",
                "Baby Blue Brew",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 0,
                ResourcesByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Add)] = new()
                    {
                        ["Stamina"]  = 25.0f
                    }
                }
            ),
            new(
                EntityCategory.Item,
                "oozing_orange_oil",
                "Oozing Orange Oil",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Add)] = new()
                    {
                        ["MaximumHealth"]  = 5.0f
                    }
                }
            ),
            new(
                EntityCategory.Item,
                "glowing_green_grog",
                "Glowing Green Grog",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Add)] = new()
                    {
                        ["MaximumStamina"]  = 5.0f
                    }
                }
            ),
            new(
                EntityCategory.Item,
                "yielding_yellow_yarb",
                "Yielding Yellow Yarb",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Add)] = new()
                    {
                        ["Armor"]  = 5.0f
                    }
                }
            ),
            new(
                EntityCategory.Item,
                "pale_purple_potion",
                "Pale Purple Potion",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Add)] = new()
                    {
                        ["Shield"]  = 5.0f
                    }
                }
            ),
            new(
                EntityCategory.Item,
                "pupil_porridge",
                "Pupil Porridge",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Add)] = new()
                    {
                        ["Critical"]  = 5.0f
                    }
                }
            ),
            new(
                EntityCategory.Item,
                "cartilage_chowder",
                "Cartilage Chowder",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Add)] = new()
                    {
                        ["Dodge"]  = 5.0f
                    }
                }
            ),
            new(
                EntityCategory.Item,
                "magpie_morsel",
                "Magpie Morsel",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Add)] = new()
                    {
                        ["Luck"]  = 5.0f
                    }
                }
            ),
            new(
                EntityCategory.Item,
                "vulture_vittles",
                "Vulture Vittles",
                ItemType.Consumable,
                ItemRarity.Common,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Add)] = new()
                    {
                        ["Peer"]  = 5.0f
                    }
                }
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
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Multiply)] = new()
                    {
                        ["MaximumHealth"]  = 1.25f
                    }
                },

                ResourcesByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Multiply)] = new()
                    {
                        ["Health"] = 1.25f
                    }
                }
            ),
            new(
                EntityCategory.Item,
                "teal_tincture",
                "Teal Tincture",
                ItemType.Consumable,
                ItemRarity.Uncommon,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Multiply)] = new()
                    {
                        ["MaximumStamina"]  = 1.25f
                    }
                },

                ResourcesByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Multiply)] = new()
                    {
                        ["Stamina"] = 1.25f
                    }
                }
            ),
            new(
                EntityCategory.Item,
                "earthy_elixir",
                "Earthy Elixir",
                ItemType.Consumable,
                ItemRarity.Uncommon,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Multiply)] = new()
                    {
                        ["Armor"]           = 1.25f,
                        ["Shield"]          = 1.25f
                    }
                }
            ),

            //     ======================================================
            //     Rare
            //     ======================================================

            new(
                EntityCategory.Item,
                "chromatic_concoction",
                "Chromatic Concoction",
                ItemType.Consumable,
                ItemRarity.Rare,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Multiply)] = new()
                    {
                        ["MaximumHealth"]   = 1.25f,
                        ["MaximumStamina"]  = 1.25f,
                        ["Armor"]           = 1.25f,
                        ["Shield"]          = 1.25f
                    }
                },

                ResourcesByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Multiply)] = new()
                    {
                        ["Health"]  = 1.25f,
                        ["Stamina"] = 1.25f
                    }
                }
            ),

            new(
                EntityCategory.Item,
                "homogenous_paste",
                "Homogenous Paste",
                ItemType.Consumable,
                ItemRarity.Rare,
                StaminaCost: 15,
                StatsByTrigger: new()
                {
                    [(Trigger.OnUse, ModificationType.Multiply)] = new()
                    {
                        ["Critical"]   = 1.25f,
                        ["Dodge"]  = 1.25f
                    }
                }
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
                EquipType: EquipType.Weapon,
                StaminaCost: 25,
                Damage: 10,
                DamageType: DamageType.Physical,
                CanCrit: true,
                CanDodge: true,
                EffectsByTrigger: new() {{(EffectAction.Apply, TargetType.Target, Trigger.OnUse), ["inferno"]}}
            ),
            new(
                EntityCategory.Item,
                "crescent",
                "Crescent",
                ItemType.Weapon,
                ItemRarity.Common,
                EquipType: EquipType.Weapon,
                StaminaCost: 25,
                Damage: 10,
                DamageType: DamageType.Physical,
                CanCrit: true,
                CanDodge: true,
                EffectsByTrigger: new()
                {
                    {(EffectAction.Remove, TargetType.Self, Trigger.OnHit), ["moonlight"] },
                    {(EffectAction.Remove, TargetType.Self, Trigger.OnUnequip), ["moonlight"] },
                    {(EffectAction.Apply, TargetType.Self, Trigger.OnTurnStartWhileEquipped), ["moonlight"] },
                    {(EffectAction.Apply, TargetType.Self, Trigger.OnEquip), ["moonlight"] },
                }
            ),

            // ==========================================================
            // Armor
            // ==========================================================
            //     ======================================================
            //     Common
            //     ======================================================

            new(
                EntityCategory.Item,
                "oculus_helmet",
                "Oculus Helmet",
                ItemType.Armor,
                ItemRarity.Common,
                EquipType: EquipType.Helmet,
                SetTag: "oculus",
                StatsByTrigger: new()
                {
                    [(Trigger.OnEquip, ModificationType.Add)] = new()
                    {
                        ["Armor"]   = 25f,
                        ["Shield"]  = 25f
                    },
                    [(Trigger.OnUnequip, ModificationType.Add)] = new()
                    {
                        ["Armor"]   = -25f,
                        ["Shield"]  = -25f
                    }
                },
                EffectsByTrigger: new()
                {
                    {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["third_eye"] },
                    {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["third_eye"] }
                }
            ),
            new(
                EntityCategory.Item,
                "oculus_chestplate",
                "Oculus Chestplate",
                ItemType.Armor,
                ItemRarity.Common,
                EquipType: EquipType.Chestplate,
                SetTag: "oculus",
                StatsByTrigger: new()
                {
                    [(Trigger.OnEquip, ModificationType.Add)] = new()
                    {
                        ["Armor"]   = 25f,
                        ["Shield"]  = 25f
                    },
                    [(Trigger.OnUnequip, ModificationType.Add)] = new()
                    {
                        ["Armor"]   = -25f,
                        ["Shield"]  = -25f
                    }
                },
                EffectsByTrigger: new()
                {
                    {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["third_eye"] },
                    {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["third_eye"] }
                }
            ),
            new(
                EntityCategory.Item,
                "oculus_leggings",
                "Oculus Leggings",
                ItemType.Armor,
                ItemRarity.Common,
                EquipType: EquipType.Leggings,
                SetTag: "oculus",
                StatsByTrigger: new()
                {
                    [(Trigger.OnEquip, ModificationType.Add)] = new()
                    {
                        ["Armor"]   = 25f,
                        ["Shield"]  = 25f
                    },
                    [(Trigger.OnUnequip, ModificationType.Add)] = new()
                    {
                        ["Armor"]   = -25f,
                        ["Shield"]  = -25f
                    }
                },
                EffectsByTrigger: new()
                {
                    {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["third_eye"] },
                    {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["third_eye"] }
                }
            ),

            // Add more templates as needed
        ];

        public static void AddComponents(Entity item, Entity player, ItemTemplate template)
        {
            // Change Item Data to not take display name
            new ItemData(item, template.DisplayName, template.Rarity, template.Type, player);

            if (template.Type == ItemType.Consumable) // Only Consumables get Consumable
                new Consumable(item);

            if (template.SetTag != null)
                new CompletesItemSet(item, template.SetTag);

            if ((template.Type == ItemType.Consumable || template.Type == ItemType.Weapon) &&
                template.StaminaCost is int staminaCost)
            {
                new Usable(item, staminaCost);
            }    // Consumables or Weapons get Usable if their StaminaCost is defined.

            if (template.Type == ItemType.Weapon)
            {
                new DealsDamage(item, template.Damage, template.DamageType, template.CanCrit);
                new Hits(item, template.CanDodge);
            }
            
            if (template.EquipType is EquipType equipType) // Non-null EquipType gets Wearable
                new Wearable(item, equipType);


            if (template.StatsByTrigger != null || template.ResourcesByTrigger != null)
            {
                new ModifiesStats(item, template.StatsByTrigger, template.ResourcesByTrigger);
            }

            if (template.EffectsByTrigger != null)
            {
                new ModifiesEffects(item, template.EffectsByTrigger);
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
