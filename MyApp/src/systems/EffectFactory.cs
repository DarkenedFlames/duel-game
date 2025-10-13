namespace CBA
{
    public class EffectEntity : Entity { }

    public static class EffectFactory
    {
        public class EffectTemplate
        {
            public string Name { get; init; } = string.Empty;
            public bool IsNegative { get; init; } = false;
            public bool IsHidden { get; init; } = false;
            public StackingType StackingType { get; init; } = StackingType.AddStack;
            public int MaxStacks { get; init; } = 1;
            public Action<Entity, Entity>? Factory { get; init; }
        }

        public static readonly List<EffectTemplate> EffectTemplates = new()
        {
            new EffectTemplate
            {
                Name = "Inferno",
                StackingType = StackingType.AddStack,
                MaxStacks = 5,
                Factory = (effectEntity, player) =>
                {
                    new EffectData(effectEntity, player, "Inferno", isNegative: true,
                        stackingType: StackingType.AddStack, maxStacks: 5);

                    new EffectDuration(effectEntity, 3);

                    var playerStats = Helper.ThisIsNotNull(
                        player.GetComponent<StatsComponent>(),
                        "EffectTemplate 'Inferno': Unexpected null value for player StatComponent."
                    );

                    new DealsDamage(
                        effectEntity,
                        getDamage: () => {return (int)((playerStats?.Get("MaximumHealth") ?? 0) * 0.01f);},
                        damageType: DamageType.Magical,
                        canCrit: false,
                        canDodge: false
                    );
                }
            },
            new EffectTemplate
            {
                Name = "Poison",
                IsNegative = true,
                StackingType = StackingType.RefreshOnly,
                Factory = (effectEntity, player) =>
                {
                    new EffectData(effectEntity, player, "Poison", isNegative: true,
                        stackingType: StackingType.RefreshOnly);

                    new EffectDuration(effectEntity, 5);
                }
            },
            new EffectTemplate
            {
                Name = "Shield",
                IsNegative = false,
                StackingType = StackingType.Ignore,
                Factory = (effectEntity, player) =>
                {
                    new EffectData(effectEntity, player, "Shield", isNegative: false,
                        stackingType: StackingType.Ignore);
                    // no duration = permanent shield
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

        public static void ApplyEffect(string name, Entity playerEntity)
        {
            var template = GetTemplate(name);

            // Find existing instance on player
            var existingEffect = World.Instance.GetEntitiesWith<EffectData>()
                .Select(e => e.GetComponent<EffectData>()!)
                .FirstOrDefault(ed => ed.PlayerEntity == playerEntity &&
                                      ed.Name == template.Name);

            // Handle stacking/refresh before creating a new one
            if (existingEffect != null)
            {
                switch (existingEffect.StackingType)
                {
                    case StackingType.RefreshOnly:
                        var dur = existingEffect.Owner.GetComponent<EffectDuration>();
                        if (dur != null) dur.Remaining = dur.Maximum;
                        return; // don’t create new

                    case StackingType.AddStack:
                        if (existingEffect.CurrentStacks < existingEffect.MaximumStacks)
                        {
                            existingEffect.CurrentStacks++;
                            var d = existingEffect.Owner.GetComponent<EffectDuration>();
                            if (d != null) d.Remaining = d.Maximum;
                        }
                        return;

                    case StackingType.Ignore:
                        return;
                }
            }

            // No existing effect → create a new one
            var effectEntity = new EffectEntity();
            template.Factory?.Invoke(effectEntity, playerEntity);
            effectEntity.SubscribeAll();
            World.Instance.AddEntity(effectEntity);
        }
    }
}