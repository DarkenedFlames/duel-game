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
        public static readonly List<EffectTemplate> Templates =
        [
            new(
                EntityCategory.Effect,
                "inferno",
                "Inferno",
                MaxStacks: 5,
                Chance: .15f,
                Components:
                [
                    (e, target) => new EffectData(e, target, true, false, maxStacks: 5),
                    (e, target) => new EffectDuration(e, 3),
                    (e, target) => new DealsDamage(e, 5, DamageType.Magical, false)
                ]

            ),

            new(
                EntityCategory.Effect,
                "moonlight",
                "Moonlight",
                StackingType: StackingType.Ignore,
                MaxPerTurn: 1,
                Chance: 1f,
                Components:
                [
                    (e, target) => new EffectData(e, target, true, false, StackingType.Ignore, maxStacks: 1),
                    (e, target) => new ModifiesStats(e,
                    StatsByTrigger: new()
                        {
                            [(Trigger.OnAdded, ModificationType.Add)] = new()
                            {
                                ["Critical"] = 100f
                            },
                            [(Trigger.OnRemoved, ModificationType.Add)] = new()
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
                MaxStacks: 1,
                Chance: 1f,
                Components:
                [
                    (e, target) => new EffectData(e, target, false, false, StackingType.Ignore, maxStacks: 1),
                    (e, target) => new ModifiesStats(e,
                    StatsByTrigger: new()
                        {
                            [(Trigger.OnAdded, ModificationType.Multiply)] = new()
                            {
                                ["Critical"] = 1.2f
                            },
                            [(Trigger.OnRemoved, ModificationType.Multiply)] = new()
                            {
                                ["Critical"] = 1.0f/1.2f
                            }
                        }
                    )
                ]
            ),
        ];
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

            effect.SubscribeAll();
            World.Instance.AddEntity(effect);
        }
    }
}