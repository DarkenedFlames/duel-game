using System;
using System.Collections.Generic;
using System.Linq;

// Needs heavy refactor... ties in with stacking and effect application

namespace CBA
{
    public class EffectEntity : Entity
    {
        // Concrete effect entity
    }

    public static class EffectFactory
    {
        public class EffectTemplate
        {
            public string Name { get; init; } = string.Empty;
            public bool IsNegative { get; init; } = false;
            public bool IsHidden { get; init; } = false;
            public StackingType StackingType { get; init; } = StackingType.AddStack;
            public int MaxStacks { get; init; } = 1;
            public int Duration { get; init; } = 1;
            public Action<Entity, Entity>? Factory { get; init; }
        }

        public static readonly List<EffectTemplate> EffectTemplates = new()
        {
            new EffectTemplate
            {
                Name = "Burn",
                IsNegative = true,
                Duration = 3,
                StackingType = StackingType.AddStack,
                MaxStacks = 5,
                Factory = (effectEntity, playerEntity) =>
                {
                    new EffectData(effectEntity, playerEntity, "Burn", isNegative: true);
                    new EffectDuration(effectEntity, 3);
                    new EffectStacking(effectEntity, StackingType.AddStack, 5);
                    // Add any Burn-specific logic components here later (e.g., DOT)
                }
            },
            new EffectTemplate
            {
                Name = "Poison",
                IsNegative = true,
                Duration = 5,
                StackingType = StackingType.RefreshOnly,
                Factory = (effectEntity, playerEntity) =>
                {
                    new EffectData(effectEntity, playerEntity, "Poison", isNegative: true);
                    new EffectDuration(effectEntity, 5);
                    new EffectStacking(effectEntity, StackingType.RefreshOnly);
                }
            },
            new EffectTemplate
            {
                Name = "Shield",
                IsNegative = false,
                Duration = 2,
                StackingType = StackingType.Ignore,
                Factory = (effectEntity, playerEntity) =>
                {
                    new EffectData(effectEntity, playerEntity, "Shield", isNegative: false);
                    new EffectDuration(effectEntity, 2);
                    new EffectStacking(effectEntity, StackingType.Ignore);
                }
            }
        };

        public static EffectTemplate GetTemplate(string name)
        {
            var template = EffectTemplates.FirstOrDefault(t =>
                t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (template == null)
                throw new Exception($"Effect template '{name}' not found.");

            return template;
        }

        public static Entity CreateEffect(string name, Entity playerEntity)
        {
            var template = GetTemplate(name);
            var effectEntity = new EffectEntity();

            // Apply template factory logic
            template.Factory?.Invoke(effectEntity, playerEntity);

            // Add to world
            World.Instance.AddEntity(effectEntity);

            return effectEntity;
        }

        public static void ApplyEffect(string name, Entity playerEntity)
        {
            var effectEntity = CreateEffect(name, playerEntity);

            // Handle stacking logic automatically via component
            var stacking = effectEntity.GetComponent<EffectStacking>();
            stacking?.HandleStacking(playerEntity);
        }
    }
}
