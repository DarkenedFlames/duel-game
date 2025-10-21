namespace CBA
{
    public class Hits(Entity owner, bool dodgeable) : Component(owner)
    {
        private readonly bool Dodgeable = dodgeable;
        public event Action<Entity, Entity>? OnHit;
        public event Action<Entity, Entity>? OnDodge;
        public event Action<Entity, Entity>? OnMiss;

        protected override void RegisterSubscriptions()
        {
            RegisterSubscription<Action<Entity, Entity>>(
                h => Owner.GetComponent<Usable>().OnUseSuccess += h,
                h => Owner.GetComponent<Usable>().OnUseSuccess -= h,
                (_, target) => TryHit(target)
            );
        }
        private void TryHit(Entity target)
        {
            StatsComponent targetStats = target.GetComponent<StatsComponent>();
            Entity hitter = World.GetPlayerOf(Owner);
            StatsComponent hitterStats = hitter.GetComponent<StatsComponent>();

            if (Dodgeable && Random.Shared.NextDouble() < targetStats.GetHyperbolic("Dodge"))
            {
                Printer.PrintDodged(Owner, target);
                OnDodge?.Invoke(Owner, target);
                return;
            }

            if(Random.Shared.NextDouble() > hitterStats.Get("Accuracy") / 100)
            {
                Printer.PrintMissed(Owner, target);
                OnMiss?.Invoke(Owner, target);
                return;
            }

            OnHit?.Invoke(Owner, target);
        }
    }
}