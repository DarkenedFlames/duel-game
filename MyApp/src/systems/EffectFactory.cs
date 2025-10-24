namespace CBA
{
    public delegate Component ComponentBuilder(Entity newEntity, Entity target);

    public record EffectTemplate
    (
        EntityCategory Category,
        string TypeId,
        string DisplayName,
        StackingType StackingType = StackingType.AddStack,
        int MaxStacks = 1,
        int? MaxPerTurn = null,
        float Chance = 1.0f,
        List<ComponentBuilder>? Components = null
    );

    public static class EffectFactory
    {
        public static EffectTemplate GetTemplate(string typeId)
        {
            return Templates.FirstOrDefault(t =>
                t.TypeId.Equals(typeId, StringComparison.OrdinalIgnoreCase))
                ?? throw new Exception($"Effect template '{typeId}' not found.");
        }
        private static EffectTemplate? GateEffect(string typeId, Entity target)
        {
            EffectTemplate template = GetTemplate(typeId);

            if (Random.Shared.NextDouble() > template.Chance)
                return null;

            if (template.MaxPerTurn != null)
                if (template.MaxPerTurn <= target.GetComponent<TurnMemory>().GetEffectsAppliedThisTurn(template.TypeId))
                    return null;

            // Handle stacking
            EffectData? existing = World.Instance
                .GetAllForPlayer<EffectData>(target, EntityCategory.Effect, template.TypeId)
                .FirstOrDefault();

            if (existing != null)
            {
                Entity existingEffect = existing.Owner;

                switch (existing.StackingType)
                {
                    case StackingType.RefreshOnly:
                        if (existingEffect.HasComponent<EffectDuration>())
                        {
                            EffectDuration effectDuration = existingEffect.GetComponent<EffectDuration>();
                            effectDuration.Remaining = effectDuration.Maximum;
                        }
                        return null;

                    case StackingType.AddStack:
                        if (existing.CurrentStacks < existing.MaximumStacks && existingEffect.HasComponent<EffectDuration>())
                        {
                            existing.CurrentStacks++;
                            EffectDuration effectDuration = existingEffect.GetComponent<EffectDuration>();
                            effectDuration.Remaining = effectDuration.Maximum;
                        }
                        return null;

                    case StackingType.Ignore:
                        return null;
                }
            }

            return template;
        }
        public static void ApplyEffect(string typeId, Entity target)
        {
            EffectTemplate? template = GateEffect(typeId, target);
            if (template == null) return;

            // Create new effect entity
            Entity effect = new(EntityCategory.Effect, template.TypeId, template.DisplayName);

            if (template.Components != null)
                foreach (ComponentBuilder builder in template.Components)
                {
                    Component component = builder(effect, target);
                    effect.AddComponent(component);
                }

            World.Instance.AddEntity(effect);
        }
        public static readonly List<EffectTemplate> Templates =
        [
            new(
                EntityCategory.Effect,
                "inferno",
                "Inferno",
                MaxStacks: 5,
                Chance: .33f,
                Components:
                [
                    (e, target) => new EffectData(e, target, true, false, maxStacks: 5),
                    (e, target) => new EffectDuration(e, 3),
                    (e, target) => new DealsDamage(e, 5, DamageType.Magical, false)
                ]

            ),
            new(
                EntityCategory.Effect,
                "sunburn",
                "Sunburn",
                MaxStacks: 5,
                Chance: .15f,
                Components:
                [
                    (e, target) => new EffectData(e, target, true, false, maxStacks: 5),
                    (e, target) => new EffectDuration(e, 3),
                    (e, target) => new DealsDamage(e, 10, DamageType.True, false)
                ]

            ),
            new(
                EntityCategory.Effect,
                "moonlight",
                "Moonlight",
                StackingType: StackingType.Ignore,
                MaxPerTurn: 1,
                Components:
                [
                    (e, target) => new EffectData(e, target, true, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Add, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Critical"] = 100f
                            },
                            [(ModificationType.Add, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Critical"] = -100f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "third_eye",
                "Third Eye",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Critical"] = 2.0f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Critical"] = 0.5f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "vibrata",
                "Vibrata",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Dodge"] = 2.0f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Dodge"] = 0.5f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "pungence",
                "Pungence",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["ConsumableCost"] = .75f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["ConsumableCost"] = 1f/.75f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "dexterity",
                "Dexterity",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Stimming"] = 2.0f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Stimming"] = 0.5f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "delight",
                "Delight",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Healing"] = 2.0f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Healing"] = 0.5f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "reflection",
                "Reflection",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Precision"] = 2.0f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Precision"] = 0.5f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "pleonexia",
                "Pleonexia",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Luck"] = 2.0f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Luck"] = 0.5f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "rage",
                "Rage",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Attack"] = 2.0f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Attack"] = 0.5f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "wanting",
                "Wanting",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Steal"] = 2.0f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Steal"] = 0.5f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "awakening",
                "Awakening",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["MaximumStamina"] = 2.0f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["MaximumStamina"] = 0.5f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "devouring",
                "Devouring",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["MaximumHealth"] = 2.0f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["MaximumHealth"] = 0.5f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "cheating",
                "Cheating",
                StackingType: StackingType.Ignore,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["WeaponCost"] = .75f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["WeaponCost"] = 1f/.75f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "crushed",
                "Crushed",
                StackingType: StackingType.Ignore,
                Chance: .15f,
                Components:
                [
                    (e, target) => new EffectData(e, target, true, false, StackingType.Ignore),
                    (e, target) => new EffectDuration(e, 1),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Critical"] = .8f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Critical"] = 1f/.8f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "blinded",
                "Blinded",
                StackingType: StackingType.Ignore,
                Chance: .15f,
                Components:
                [
                    (e, target) => new EffectData(e, target, true, false, StackingType.Ignore),
                    (e, target) => new EffectDuration(e, 1),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Critical"] = .8f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Critical"] = 1f/.8f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "frozen",
                "Frozen",
                StackingType: StackingType.Ignore,
                Chance: .15f,
                Components:
                [
                    (e, target) => new EffectData(e, target, true, false, StackingType.Ignore),
                    (e, target) => new EffectDuration(e, 2),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["MaximumStamina"] = .8f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["MaximumStamina"] = 1f/.8f
                            }
                        }
                    )
                ]
            ),
            new(
                EntityCategory.Effect,
                "noons_visibility",
                "Noon's Visibility",
                StackingType: StackingType.Ignore,
                Chance: .15f,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore),
                    (e, target) => new EffectDuration(e, 2),
                    (e, target) => new ModifiesStats(e,
                    statsByTrigger: new()
                        {
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnAdded)] = new()
                            {
                                ["Critical"] = 1.2f
                            },
                            [(ModificationType.Multiply, TargetType.Self, Trigger.OnRemoved)] = new()
                            {
                                ["Critical"] = 1f/1.2f
                            }
                        }
                    )
                ]
            ),
            // Add more templates as needed
        ];
    }
}