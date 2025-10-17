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
        float Chance = 1.0f,
        StackingType StackingType = StackingType.AddStack,
        int MaxStacks = 1,
        ModifiesStatsTrigger? StatsTrigger = null,
        Dictionary<string, int>? StatAdditions = null,
        Dictionary<string, int>? ResourceAdditions = null,
        Dictionary<string, float>? StatModifiers = null,
        Dictionary<string, float>? ResourceModifiers = null,
        Dictionary<EffectTrigger, List<string>>? EffectsByTrigger = null,
        int Damage = 0,
        DamageType DamageType = DamageType.Physical,
        bool CanCrit = true,
        bool CanDodge = true

    );

    public static class EffectFactory
    {
        public static event Action<Entity, Entity>? OnEffectApplied;

        public static readonly List<EffectTemplate> Templates = new()
        {
            new(
                EntityCategory.Effect,
                "inferno",
                "Inferno",
                IsNegative: true,
                IsHidden: false,
                Chance: .15f,
                StackingType: StackingType.AddStack,
                MaxStacks: 5,
                Damage: 1, // 1% health per turn
                DamageType: DamageType.Magical,
                CanCrit: false,
                CanDodge: false
            ),

            new(
                EntityCategory.Effect,
                "poison",
                "Poison",
                IsNegative: true,
                IsHidden: false,
                StackingType: StackingType.RefreshOnly,
                MaxStacks: 1,
                Damage: 5,
                DamageType: DamageType.Physical
            ),

            new(
                EntityCategory.Effect,
                "shield",
                "Shield",
                IsNegative: false,
                IsHidden: false,
                StackingType: StackingType.Ignore
            )
        };

        public static EffectTemplate GetTemplate(string typeId)
        {
            return Templates.FirstOrDefault(t =>
                t.TypeId.Equals(typeId, StringComparison.OrdinalIgnoreCase))
                ?? throw new Exception($"Effect template '{typeId}' not found.");
        }

        public static void ApplyEffect(string typeId, Entity target)
        {
            EffectTemplate template = GetTemplate(typeId);

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
                template.MaxStacks
            );

            // Duration (e.g., Inferno and Poison)
            if (template.TypeId is "inferno" or "poison")
                new EffectDuration(effect, 3);

            // Damage-over-time effects
            if (template.Damage > 0)
            {
                StatsComponent? playerStats = player.GetComponent<StatsComponent>();
                int damageValue = template.DisplayName == "Inferno"
                    ? (int)((playerStats?.Get("MaximumHealth") ?? 0) * 0.01f)
                    : template.Damage;

                new DealsDamage(
                    effect,
                    getDamage: () => damageValue,
                    damageType: template.DamageType,
                    canCrit: template.CanCrit,
                    canDodge: template.CanDodge
                );
            }

            // Future: apply stat/resource mods, triggers, etc.
        }
    }
}