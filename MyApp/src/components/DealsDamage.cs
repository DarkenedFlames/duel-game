using System.Diagnostics.CodeAnalysis;

namespace CBA
{
    public class DealsDamage : Component
    {
        private readonly Func<int>? _getDamage; // dynamic damage function
        private readonly int _staticDamage;     // static damage fallback

        public int Damage => _getDamage?.Invoke() ?? _staticDamage;

        public DamageType DamageType { get; init; } = DamageType.Physical;
        public bool CanCrit { get; init; }
        public bool CanDodge { get; init; }

        public event Action<Entity, Entity, int>? OnDamageDealt;

        // --- Static damage constructor ---
        public DealsDamage(Entity owner, int damage,
                           DamageType damageType = DamageType.Physical,
                           bool canCrit = false,
                           bool canDodge = false) : base(owner)
        {
            _staticDamage = damage;
            DamageType = damageType;
            CanCrit = canCrit;
            CanDodge = canDodge;
        }

        // --- Dynamic damage constructor ---
        public DealsDamage(Entity owner, Func<int> getDamage,
                           DamageType damageType = DamageType.Physical,
                           bool canCrit = false,
                           bool canDodge = false) : base(owner)
        {
            _getDamage = getDamage ?? throw new ArgumentNullException(nameof(getDamage));
            DamageType = damageType;
            CanCrit = canCrit;
            CanDodge = canDodge;
        }

        public override void Subscribe()
        {
            OnDamageDealt += Printer.PrintDamageDealt;

            Usable? usable = Owner.GetComponent<Usable>();
            EffectData? effectData = Owner.GetComponent<EffectData>();
            Helper.NotAllAreNull
            (
                $"DealsDamage.Subscribe() requires Usable or EffectData: Source: {Owner}",
                usable,
                effectData
            );

            // --- Item logic ---
            usable?.OnUseSuccess += ApplyDamage;

            // --- Effect logic ---
            World.Instance.TurnManager.OnTurnStart += (player) =>
            {
                if (player == effectData?.PlayerEntity) ApplyDamage(Owner, player);
            };
        }

        private void ApplyDamage(Entity itemOrEffect, Entity target)
        {
            int finalDamage = Damage;

            StatsComponent targetStats = Helper.ThisIsNotNull
            (
                target.GetComponent<StatsComponent>(),
                $"DealsDamage.ApplyDamage: Unexpected null value for target's StatsComponent."
            );

            ResourcesComponent targetResources = Helper.ThisIsNotNull
            (
                target.GetComponent<ResourcesComponent>(),
                $"DealsDamage.ApplyDamage: Unexpected null value for target's ResourcesComponent."
            );

            // --- Identify source type ---
            bool isItem = itemOrEffect.HasComponent<ItemData>();
            bool isEffect = itemOrEffect.HasComponent<EffectData>();

            // Only items have "users" (attackers)
            StatsComponent? userStats = null;

            // --- Dodge (applies to both) ---
            if (CanDodge && Random.Shared.NextDouble() < targetStats.GetHyperbolic("Dodge"))
            {
                Printer.PrintDodged(Owner, target);
                finalDamage = 0;
            }

            if (finalDamage > 0)
            {
                // --- Crit (items only) ---
                if (isItem && CanCrit)
                {
                    userStats = Helper.ThisIsNotNull
                    (
                        World.Instance.GetPlayerOf(itemOrEffect)?.GetComponent<StatsComponent>(),
                        "DealsDamage.ApplyDamage: Unexpected null value for user's StatsComponent."
                    );

                    if (Random.Shared.NextDouble() < userStats.GetHyperbolic("Critical"))
                    {
                        finalDamage = (int)(finalDamage * 2.0f);
                        Printer.PrintCritical(Owner, target);
                    }
                }

                // --- Damage Reduction ---
                float divisor = DamageType switch
                {
                    DamageType.Physical => targetStats.GetHyperbolic("Armor"),
                    DamageType.Magical  => targetStats.GetHyperbolic("Shield"),
                    _                   => 1f
                };

                finalDamage = (int)(finalDamage / divisor);

                // --- Apply Damage ---
                targetResources.Change("Health", -finalDamage);
                OnDamageDealt?.Invoke(itemOrEffect, target, finalDamage);
            }
        }
    }
}