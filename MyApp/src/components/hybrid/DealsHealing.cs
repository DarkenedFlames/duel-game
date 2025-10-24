namespace CBA
{
    public class DealsHealing(
        Entity owner,
        int healing
    ) : Component(owner)
    {
        public int Healing = healing;

        public event Action<Entity, Entity, int>? OnHealingDone;

        protected override void RegisterSubscriptions()
        {
            switch (Owner.Id.Category)
            {
                case EntityCategory.Item:
                    var usable = Owner.GetComponent<Usable>();
                    RegisterSubscription<Action<Entity, Entity>>(
                        h => usable.OnUseSuccess += h,
                        h => usable.OnUseSuccess -= h,
                        (_, target) => ApplyHealing(target)
                    );
                    break;

                case EntityCategory.Effect:
                    RegisterSubscription<Action<Entity>>(
                        h => World.Instance.TurnManager.OnTurnStart += h,
                        h => World.Instance.TurnManager.OnTurnStart -= h,
                        ApplyHealing
                    );
                    break;
            }
        }

        private void ApplyHealing(Entity target)
        {
            if (Owner.Id.Category == EntityCategory.Effect && target != World.GetPlayerOf(Owner))
                return;

            float floatHealing = Healing;

            floatHealing *= target.GetComponent<StatsComponent>().GetLinearClamped("Healing", .25f);

            int finalHealing = (int)floatHealing;

            target.GetComponent<ResourcesComponent>().Change("Health", finalHealing);

            OnHealingDone?.Invoke(Owner, target, finalHealing);
        }
    }
}
