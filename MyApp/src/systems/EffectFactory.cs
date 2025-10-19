using System;
using System.Collections.Generic;
using System.Linq;

namespace CBA
{
    public record EffectTemplate
    (
        EntityCategory Category,
        string TypeId,
        string DisplayName,
        bool IsNegative,
        bool IsHidden,
        int? MaxPerTurn = null,
        int Duration = 0,
        float Chance = 1.0f,
        StackingType StackingType = StackingType.AddStack,
        int MaxStacks = 1,
        Dictionary<(Trigger, ModificationType), Dictionary<string, float>>? StatsByTrigger = null,
        Dictionary<(Trigger, ModificationType), Dictionary<string, float>>? ResourcesByTrigger = null,
        int Damage = 0,
        DamageType DamageType = DamageType.Physical,
        bool CanCrit = true

    );

    public static class EffectFactory
    {
        public static event Action<Entity, Entity>? OnEffectApplied;
        public static readonly List<EffectTemplate> Templates =
        [
            new(
                EntityCategory.Effect,
                "inferno",
                "Inferno",
                IsNegative: true,
                IsHidden: false,
                Duration: 3,
                Chance: .15f,
                StackingType: StackingType.AddStack,
                MaxStacks: 5,
                Damage: 1, // 1% health per turn
                DamageType: DamageType.Magical,
                CanCrit: false
            ),

            new(
                EntityCategory.Effect,
                "moonlight",
                "Moonlight",
                IsNegative: true,
                IsHidden: true,
                MaxPerTurn: 1,
                Duration: 1,
                Chance: 1f,
                StackingType: StackingType.Ignore,
                MaxStacks: 1,
                Damage: 0,
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
            ),
            new(
                EntityCategory.Effect,
                "third_eye",
                "Third Eye",
                IsNegative: false,
                IsHidden: false,
                Chance: 1f,
                StackingType: StackingType.Ignore,
                MaxStacks: 1,
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
            ),
        ];
        public static EffectTemplate GetTemplate(string typeId)
        {
            return Templates.FirstOrDefault(t =>
                t.TypeId.Equals(typeId, StringComparison.OrdinalIgnoreCase))
                ?? throw new Exception($"Effect template '{typeId}' not found.");
        }
        public static void ApplyEffect(string typeId, Entity target)
        {
            EffectTemplate template = GetTemplate(typeId);

            if (Random.Shared.NextDouble() > template.Chance)
                return;

            if (template.MaxPerTurn != null)
                if (template.MaxPerTurn <= target.GetComponent<TurnMemory>().GetEffectsAppliedThisTurn(template.TypeId))
                    return;

            // Handle stacking
            EffectData? existing = World.Instance
                .GetAllForPlayer<EffectData>(target, EntityCategory.Effect, template.TypeId)
                .FirstOrDefault();

            if (existing != null)
            {
                Entity existingEffect = existing.Owner;

                switch (existing?.StackingType)
                {
                    case StackingType.RefreshOnly:
                        if (existingEffect.HasComponent<EffectDuration>())
                        {
                            EffectDuration effectDuration = existingEffect.GetComponent<EffectDuration>();
                            effectDuration.Remaining = effectDuration.Maximum;
                        }
                        return;

                    case StackingType.AddStack:
                        if (existing.CurrentStacks < existing.MaximumStacks && existingEffect.HasComponent<EffectDuration>())
                        {
                            existing.CurrentStacks++;
                            EffectDuration effectDuration = existingEffect.GetComponent<EffectDuration>();
                            effectDuration.Remaining = effectDuration.Maximum;
                        }
                        return;

                    case StackingType.Ignore:
                        return;
                }
            }
            // Create new effect entity
            Entity effect = new(EntityCategory.Effect, template.TypeId, template.DisplayName);
            AddComponents(effect, template, target);
            World.Instance.AddEntity(effect);
            OnEffectApplied?.Invoke(target, effect);
        }
        private static void AddComponents(Entity effect, EffectTemplate template, Entity player)
        {
            // Core data
            new EffectData(
                effect,
                player,
                template.DisplayName,
                template.IsNegative,
                template.IsHidden,
                template.StackingType,
                template.MaxStacks,
                template.MaxPerTurn
            );

            // Duration
            if (template.Duration > 0)
                new EffectDuration(effect, template.Duration);

            // Damage-over-time effects
            if (template.Damage > 0)
            {
                StatsComponent playerStats = player.GetComponent<StatsComponent>();
                int damageValue = template.DisplayName == "Inferno"
                    ? (int)(playerStats.Get("MaximumHealth") * 0.01f)
                    : template.Damage;

                new DealsDamage(
                    effect,
                    getDamage: () => damageValue,
                    damageType: template.DamageType,
                    canCrit: template.CanCrit
                );
            }

            if (template.StatsByTrigger != null || template.ResourcesByTrigger != null)
                new ModifiesStats(effect, template.StatsByTrigger, template.ResourcesByTrigger);
        }
    }
}