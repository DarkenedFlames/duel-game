namespace CBA
{
    public class Hits(Entity owner, bool dodgeable) : Component(owner)
    {
        public bool Dodgeable = dodgeable;
        public event Action<Entity, Entity>? OnHit;
        public event Action<Entity, Entity>? OnDodge;

        public override void Subscribe()
        {
            Owner.GetComponent<Usable>().OnUseSuccess += (_, target) => TryHit(Owner, target);
        }
        public void TryHit(Entity item, Entity target)
        {
            StatsComponent targetStats = target.GetComponent<StatsComponent>();

            if (Dodgeable && Random.Shared.NextDouble() < targetStats.GetHyperbolic("Dodge"))
            {
                Printer.PrintDodged(Owner, target);
                OnDodge?.Invoke(Owner, target);
                return;
            }

            OnHit?.Invoke(item, target);
        }
    }
}