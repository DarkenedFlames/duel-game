namespace CBA
{
    public class Hits(Entity owner, bool dodgeable) : Component(owner)
    {
        public int TimesHitThisTurn = 0;
        public int TimesHitThisRound = 0;
        public bool Dodgeable = dodgeable;

        public event Action<Entity, Entity>? OnHit;
        public event Action<Entity, Entity>? OnDodge;
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += _ => TimesHitThisTurn = 0;
            World.Instance.TurnManager.OnTurnStart += player =>
            {
                if (player == World.Instance.GetPlayerOf(Owner)) TimesHitThisRound = 0;
            };

            OnHit += (_, _) => TimesHitThisTurn++;
            OnHit += (_, _) => TimesHitThisRound++;
            Owner.GetComponent<Usable>().OnUseSuccess += (_, target) => TryHit(Owner, target);

            OnDodge += Printer.PrintDodged;
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


        public override void ValidateDependencies(){}
    }
}