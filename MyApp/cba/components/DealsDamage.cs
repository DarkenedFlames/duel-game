using System;

namespace CBA
{
    public class DealsDamage : Component
    {
        private readonly Func<int>? _getDamage; // dynamic damage function
        private readonly int _staticDamage;      // static damage fallback

        public int Damage => _getDamage?.Invoke() ?? _staticDamage;

        public DamageType DamageType { get; init; } = DamageType.Physical;
        public bool CanCrit { get; init; } = false;
        public bool CanDodge { get; init; } = false;

        public event Action<Entity, Entity, int>? OnDamageDealt;

        // Static damage constructor
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

        // Dynamic damage constructor
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

            // --- Item logic ---
            var usable = Owner.GetComponent<Usable>();
            if (usable != null)
            {
                usable.OnUseSuccess += (item, target) =>
                {
                    ApplyDamage(item, target);
                };
            }

            // --- Effect logic ---
            var effectData = Owner.GetComponent<EffectData>();
            if (effectData != null)
            {
                var player = effectData.PlayerEntity;
                if (player != null)
                {
                    var takesTurns = player.GetComponent<TakesTurns>();
                    if (takesTurns != null)
                    {
                        takesTurns.OnTurnStart += _ =>
                        {
                            ApplyDamage(Owner, player); // self-targeting effect
                        };
                    }
                }
            }
        }

        private void ApplyDamage(Entity source, Entity target)
        {
            if (target == null) return;

            int finalDamage = Damage;
            var targetStats = target.GetComponent<StatsComponent>();
            var targetResources = target.GetComponent<ResourcesComponent>();
            var userStats = source.GetComponent<StatsComponent>();

            // --- Dodge ---
            if (CanDodge && targetStats != null)
            {
                float dodgeChance = targetStats.Get("Dodge") / (targetStats.Get("Dodge") + 100f);
                if (Random.Shared.NextDouble() < dodgeChance)
                {
                    Printer.PrintDodged(Owner, target);
                    finalDamage = 0;
                }
            }

            // --- Crit ---
            if (CanCrit && userStats != null && finalDamage > 0)
            {
                float critChance = userStats.Get("Critical") / (userStats.Get("Critical") + 100f);
                if (Random.Shared.NextDouble() < critChance)
                {
                    finalDamage = (int)(finalDamage * 2.0f);
                    Printer.PrintCritical(Owner, target);
                }
            }

            // --- Damage Reduction ---
            if (targetStats != null && finalDamage > 0)
            {
                float reduction = 1f;
                switch (DamageType)
                {
                    case DamageType.Physical:
                        reduction = 100f / (targetStats.Get("Armor") + 100f);
                        break;
                    case DamageType.Magical:
                        reduction = 100f / (targetStats.Get("Shield") + 100f);
                        break;
                }

                finalDamage = (int)(finalDamage * reduction);
            }

            // --- Apply ---
            if (finalDamage > 0)
            {
                targetResources?.Change("Health", -finalDamage);
                Printer.PrintDamageDealt(Owner, target, finalDamage);
            }

            OnDamageDealt?.Invoke(source, target, finalDamage);
        }
    }
}
