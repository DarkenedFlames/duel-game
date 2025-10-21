namespace CBA
{
    public enum DamageType{ Physical, Magical, True}
    public class DealsDamage(
                            Entity owner,
                            int damage,
                            DamageType damageType = DamageType.Physical,
                            bool canCrit = false
                            ) : Component(owner)
    {
        public int Damage = damage;
        public DamageType DamageType { get; init; } = damageType;
        public bool CanCrit { get; init; } = canCrit;

        public event Action<Entity, Entity, int>? OnDamageDealt;
        public event Action<Entity, Entity>? OnCritical;

        protected override void RegisterSubscriptions()
        {
            switch (Owner.Id.Category)
            {
                case EntityCategory.Item:
                    RegisterSubscription<Action<Entity, Entity>>(
                        h => Owner.GetComponent<Hits>().OnHit += h,
                        h => Owner.GetComponent<Hits>().OnHit -= h,
                        (_, target) => ApplyDamage(target)
                    );
                    break;
                case EntityCategory.Effect:
                    RegisterSubscription<Action<Entity>>(
                        h => World.Instance.TurnManager.OnTurnStart += h,
                        h => World.Instance.TurnManager.OnTurnStart -= h,
                        ApplyDamage
                    );
                    break;
                default:
                    break;
            }
        }
        private void ApplyDamage(Entity target)
        {
            if (Owner.Id.Category == EntityCategory.Effect)
                if (target != World.GetPlayerOf(Owner))
                    return;

            int finalDamage = Damage;

            StatsComponent targetStats = target.GetComponent<StatsComponent>();
            ResourcesComponent targetResources = target.GetComponent<ResourcesComponent>();

            if (Owner.Id.Category == EntityCategory.Item && CanCrit)
            {
                StatsComponent userStats = World.GetPlayerOf(Owner).GetComponent<StatsComponent>();

                // --- Attack ---
                float attackMultiplier = 1 + userStats.GetHyperbolic("Attack");
                finalDamage = (int)(finalDamage * attackMultiplier);

                // --- Crit ---
                float critChance = userStats.GetHyperbolic("Critical");
                float critMultiplier = 2 + userStats.GetHyperbolic("Precision");
                if (Random.Shared.NextDouble() < critChance)
                {
                    finalDamage = (int)(finalDamage * critMultiplier);
                    Printer.PrintCritical(Owner, target);
                    OnCritical?.Invoke(Owner, target);
                }
            }

            // --- Damage Reduction ---
            float resistanceMultiplier = DamageType switch
            {
                DamageType.Physical => 1 - targetStats.GetHyperbolic("Armor"),
                DamageType.Magical  => 1 - targetStats.GetHyperbolic("Shield"),
                _                   => 1f
            };

            finalDamage = (int)(finalDamage * resistanceMultiplier);

            // --- Defense ---
            if (DamageType == DamageType.Physical || DamageType == DamageType.Magical)
            {
                float defenseDivisor = 1 + targetStats.GetHyperbolic("Defense");
                finalDamage = (int)(finalDamage / defenseDivisor);
            }


            // --- Apply Damage ---
            targetResources.Change("Health", -finalDamage);
            Printer.PrintDamageDealt(Owner, target, finalDamage);
            OnDamageDealt?.Invoke(Owner, target, finalDamage);
        }
    }
}