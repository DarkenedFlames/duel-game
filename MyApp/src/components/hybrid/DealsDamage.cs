namespace CBA
{
    public class DealsDamage(Entity owner, int damage,
                       DamageType damageType = DamageType.Physical,
                       bool canCrit = false) : Component(owner)
    {
        public int Damage = damage;
        public DamageType DamageType { get; init; } = damageType;
        public bool CanCrit { get; init; } = canCrit;

        public event Action<Entity, Entity, int>? OnDamageDealt;
        public event Action<Entity, Entity>? OnCritical;

        public override void Subscribe()
        {
            OnDamageDealt += Printer.PrintDamageDealt;

            switch (Owner.Id.Category)
            {
                case EntityCategory.Item:
                    Owner.GetComponent<Hits>().OnHit += ApplyDamage;
                    break;
                case EntityCategory.Effect:
                    World.Instance.TurnManager.OnTurnStart += turnTaker =>
                    {
                        if (turnTaker == World.Instance.GetPlayerOf(Owner))
                            ApplyDamage(Owner, turnTaker);
                    };
                    break;
                default:
                    break;
            }
        }
        private void ApplyDamage(Entity itemOrEffect, Entity target)
        {
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
            float multiplier = DamageType switch
            {
                DamageType.Physical => 1 - targetStats.GetHyperbolic("Armor"),
                DamageType.Magical  => 1 - targetStats.GetHyperbolic("Shield"),
                _                   => 1f
            };

            finalDamage = (int)(finalDamage * multiplier);

                // --- Apply Damage ---
            targetResources.Change("Health", -finalDamage);
            OnDamageDealt?.Invoke(itemOrEffect, target, finalDamage);
        }
    }
}