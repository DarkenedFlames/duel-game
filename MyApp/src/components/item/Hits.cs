namespace CBA
{
    public class Hits(Entity owner, bool dodgeable) : Component(owner)
    {
        public bool Dodgeable = dodgeable;
        public event Action<Entity, Entity>? OnHit;
        public event Action<Entity, Entity>? OnDodge;
        public event Action<Entity, Entity>? OnMiss;

        public override void Subscribe()
        {
            Owner.GetComponent<Usable>().OnUseSuccess += (_, target) => TryHit(target);
        }
        public void TryHit(Entity target)
        {
            StatsComponent targetStats = target.GetComponent<StatsComponent>();
            Entity hitter = World.Instance.GetPlayerOf(Owner);
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