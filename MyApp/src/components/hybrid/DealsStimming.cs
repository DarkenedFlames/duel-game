namespace CBA
{
    public class DealsStimming(
        Entity owner,
        int stimming
    ) : Component(owner)
    {
        public int Stimming = stimming;

        public event Action<Entity, Entity, int>? OnStimmingDone;

        protected override void RegisterSubscriptions()
        {
            switch (Owner.Id.Category)
            {
                case EntityCategory.Item:
                    var usable = Owner.GetComponent<Usable>();
                    RegisterSubscription<Action<Entity, Entity>>(
                        h => usable.OnUseSuccess += h,
                        h => usable.OnUseSuccess -= h,
                        (_, target) => ApplyStimming(target)
                    );
                    break;

                case EntityCategory.Effect:
                    RegisterSubscription<Action<Entity>>(
                        h => World.Instance.TurnManager.OnTurnStart += h,
                        h => World.Instance.TurnManager.OnTurnStart -= h,
                        ApplyStimming
                    );
                    break;
            }
        }

        private void ApplyStimming(Entity target)
        {
            if (Owner.Id.Category == EntityCategory.Effect && target != World.GetPlayerOf(Owner))
                return;

            int stimming = Stimming;

            stimming *= target.GetComponent<StatsComponent>().Get("Stimming") / 100;

            target.GetComponent<ResourcesComponent>().Change("Stamina", stimming);

            OnStimmingDone?.Invoke(Owner, target, stimming);
        }
    }
}
