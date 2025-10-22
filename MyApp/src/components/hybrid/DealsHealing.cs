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

            int healing = Healing;

            healing *= target.GetComponent<StatsComponent>().Get("Healing") / 100;

            target.GetComponent<ResourcesComponent>().Change("Health", healing);

            OnHealingDone?.Invoke(Owner, target, healing);
        }
    }
}
