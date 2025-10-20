using System.ComponentModel.Design.Serialization;

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
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                    ResourcesByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Add)] = new()
                            {
                                ["Health"]  = 25.0f
                            }
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
                    (e, holder) => new Usable(e, 0),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                    ResourcesByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Add)] = new()
                            {
                                ["Stamina"]  = 25.0f
                            }
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
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                    StatsByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Add)] = new()
                            {
                                ["MaximumHealth"]  = 5.0f
                            }
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
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                    StatsByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Add)] = new()
                            {
                                ["MaximumStamina"]  = 5.0f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "yielding_yellow_yarb",
                "Yielding Yellow Yarb",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                    StatsByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Add)] = new()
                            {
                                ["Armor"]  = 5.0f
                            }
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
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                    StatsByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Add)] = new()
                            {
                                ["Shield"]  = 5.0f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "pupil_porridge",
                "Pupil Porridge",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                    StatsByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Add)] = new()
                            {
                                ["Critical"]  = 5.0f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "cartilage_chowder",
                "Cartilage Chowder",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                    StatsByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Add)] = new()
                            {
                                ["Dodge"]  = 5.0f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "magpie_morsel",
                "Magpie Morsel",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                    StatsByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Add)] = new()
                            {
                                ["Luck"]  = 5.0f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "vulture_vittles",
                "Vulture Vittles",
                ItemType.Consumable,
                ItemRarity.Common,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Common, ItemType.Consumable),
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                    StatsByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Add)] = new()
                            {
                                ["Peer"]  = 5.0f
                            }
                        }
                    )
                ]
            ),
            #endregion
            //     ======================================================
            //     Uncommon
            //     ======================================================
            #region
            new(
                EntityCategory.Item,
                "auburn_amalgam",
                "Auburn Amalgam",
                ItemType.Consumable,
                ItemRarity.Uncommon,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Uncommon, ItemType.Consumable),
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
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
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "teal_tincture",
                "Teal Tincture",
                ItemType.Consumable,
                ItemRarity.Uncommon,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Uncommon, ItemType.Consumable),
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
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
                    )
                ]
            ),
            new(
                EntityCategory.Item,
                "earthy_elixir",
                "Earthy Elixir",
                ItemType.Consumable,
                ItemRarity.Uncommon,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Uncommon, ItemType.Consumable),
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Multiply)] = new()
                            {
                                ["Armor"]  = 1.25f,
                                ["Shield"] = 1.25f
                            }
                        }
                    )
                ]
            ),
            #endregion
            //     ======================================================
            //     Rare
            //     ======================================================
            #region
            new(
                EntityCategory.Item,
                "chromatic_concoction",
                "Chromatic Concoction",
                ItemType.Consumable,
                ItemRarity.Rare,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Rare, ItemType.Consumable),
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
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
                    )
                ]
            ),

            new(
                EntityCategory.Item,
                "homogenous_paste",
                "Homogenous Paste",
                ItemType.Consumable,
                ItemRarity.Rare,
                Components:
                [
                    (e, holder) => new ItemData(e, holder, ItemRarity.Rare, ItemType.Consumable),
                    (e, holder) => new Usable(e, 15),
                    (e, holder) => new Consumable(e),
                    (e, holder) => new ModifiesStats(e,
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnUse, ModificationType.Multiply)] = new()
                            {
                                ["Critical"]   = 1.25f,
                                ["Dodge"]      = 1.25f
                            }
                        }
                    )
                ]
            ),
            #endregion
            // ==========================================================
            // Weapons
            // ==========================================================
            //     ======================================================
            //     Common
            //     ======================================================
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
                        triggeredEffects: new()
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
                        triggeredEffects: new()
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
                        triggeredEffects: new()
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
                        triggeredEffects: new()
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
                        triggeredEffects: new()
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
                        triggeredEffects: new()
                        {
                            {(EffectAction.Apply, TargetType.Target, Trigger.OnHit), ["frozen"] }
                        }
                    )
                ]
            ),
            #endregion
            // ==========================================================
            // Armor
            // ==========================================================
            //     ======================================================
            //     Common
            //     ======================================================
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["substance"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["substance"] }
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["substance"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["substance"] }
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["substance"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["substance"] }
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
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
                        StatsByTrigger: new()
                        {
                            [(Trigger.OnEquip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = 25f
                            },
                            [(Trigger.OnUnequip, ModificationType.Add)] = new()
                            {
                                ["Defense"]   = -25f
                            }
                        }
                    ),
                    (e, holder) => new ModifiesEffects(e,
                        triggeredEffects: new()
                        {
                            {(EffectAction.Apply, TargetType.Self, Trigger.OnArmorSetCompleted), ["rage"] },
                            {(EffectAction.Remove, TargetType.Self, Trigger.OnArmorSetBroken), ["rage"] }
                        }
                    )
                ]
            ),
            #endregion
            #endregion
            // Add more templates as needed
        ];

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
        public static void CreateRandomItem(Entity holder)
        {
            ItemTemplate template = GetRandomTemplate();

            Entity item = new(template.Category, template.TypeId, template.DisplayName);

            if (template.Components != null)
                foreach (ComponentBuilder builder in template.Components)
                {
                    Component component = builder(item, holder);
                    item.AddComponent(component);
                }

            item.SubscribeAll();
            World.Instance.AddEntity(item);
        }
    }
}
