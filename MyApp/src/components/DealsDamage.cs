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

            // --- Item logic ---
            Owner.GetComponent<Usable>()?.OnUseSuccess += ApplyDamage;

            // --- Effect logic ---
            var player = Owner.GetComponent<EffectData>()?.PlayerEntity;
            player?.GetComponent<TakesTurns>()?.OnTurnStart += _ =>
                ApplyDamage(Owner, player); // self-targeting effect
        }

        private void ApplyDamage(Entity source, Entity? target)
        {
            if (target == null) return;

            int finalDamage = Damage;

            var targetStats = target.GetComponent<StatsComponent>();
            var targetResources = target.GetComponent<ResourcesComponent>();
            var userStats = source.GetComponent<StatsComponent>();

            // --- Dodge ---
            if (CanDodge && targetStats != null)
            {
                if (Random.Shared.NextDouble() < targetStats.GetHyperbolic("Dodge"))
                {
                    Printer.PrintDodged(Owner, target);
                    finalDamage = 0;
                }
            }

            // --- Crit ---
            if (CanCrit && userStats != null && finalDamage > 0)
            {
                
                if (Random.Shared.NextDouble() < userStats.GetHyperbolic("Critical"))
                {
                    finalDamage = (int)(finalDamage * 2.0f);
                    Printer.PrintCritical(Owner, target);
                }
            }

            // --- Damage Reduction ---
            if (targetStats != null && finalDamage > 0)
            {
                float divisor = DamageType switch
                {
                    DamageType.Physical => targetStats.GetHyperbolic("Armor"),
                    DamageType.Magical  => targetStats.GetHyperbolic("Shield"),
                    _                   => 1f
                };

                finalDamage = (int)(finalDamage / divisor);
            }

            // --- Apply Damage ---
            if (finalDamage > 0)
            {
                targetResources?.Change("Health", -finalDamage);
                Printer.PrintDamageDealt(Owner, target, finalDamage);
            }

            OnDamageDealt?.Invoke(source, target, finalDamage);
        }
    }
}