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

        public event Action<Entity, Entity, int>? OnDamageDealt;
        public event Action<Entity, Entity>? OnCritical;

        // --- Static damage constructor ---
        public DealsDamage(Entity owner, int damage,
                           DamageType damageType = DamageType.Physical,
                           bool canCrit = false) : base(owner)
        {
            _staticDamage = damage;
            DamageType = damageType;
            CanCrit = canCrit;
        }

        // --- Dynamic damage constructor ---
        public DealsDamage(Entity owner, Func<int> getDamage,
                           DamageType damageType = DamageType.Physical,
                           bool canCrit = false) : base(owner)
        {
            _getDamage = getDamage ?? throw new ArgumentNullException(nameof(getDamage));
            DamageType = damageType;
            CanCrit = canCrit;
        }

        public override void ValidateDependencies()
        {
            // Validate Owner Category
            if (Owner.Id.Category != EntityCategory.Item && Owner.Id.Category != EntityCategory.Effect)
                throw new InvalidOperationException($"{Owner.Id} was given an incompatible Component: DealsDamage.");

            // Validate Component Dependencies
            if (Owner.Id.Category == EntityCategory.Item && !Owner.HasComponent<Usable>())
                throw new InvalidOperationException($"Component Missing a Dependency: (Owner: {Owner.Id}, Component: DealsDamage, Dependency: Usable.");
        }
        public override void Subscribe()
        {
            OnDamageDealt += Printer.PrintDamageDealt;

            switch (Owner.Id.Category)
            {
                case EntityCategory.Item:
                    Owner.GetComponent<Hits>().OnHit += ApplyDamage;
                    break;
                case EntityCategory.Effect:
                    World.Instance.TurnManager.OnTurnStart += player =>
                    {
                        if (player == World.Instance.GetPlayerOf(Owner))
                            ApplyDamage(Owner, player);
                    };
                    break;
                default:
                    break;
            }
        }
        private void ApplyDamage(Entity itemOrEffect, Entity target)
        {
            if (target.Id.Category != EntityCategory.Player)
            {
                throw new InvalidOperationException($"{Owner.Id} was given an non-player target for DealsDamage.ApplyDamage.");
            }

            int finalDamage = Damage;

            StatsComponent targetStats = target.GetComponent<StatsComponent>();
            ResourcesComponent targetResources = target.GetComponent<ResourcesComponent>();

            // --- Crit (items only) ---
            if (Owner.Id.Category == EntityCategory.Item && CanCrit)
            {
                StatsComponent userStats = World.Instance.GetPlayerOf(itemOrEffect).GetComponent<StatsComponent>();
                if (Random.Shared.NextDouble() < userStats.GetHyperbolic("Critical"))
                {
                    finalDamage = (int)(finalDamage * 2.0f);
                    Printer.PrintCritical(Owner, target);
                    OnCritical?.Invoke(Owner, target);
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