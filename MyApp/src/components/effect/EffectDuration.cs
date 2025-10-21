namespace CBA
{
    public class EffectDuration(Entity owner, int maxDuration) : Component(owner)
    {
        public int Remaining { get; set; } = maxDuration;
        public int Maximum { get; set; } = maxDuration;

        public override void Subscribe()
        {
            TrackSubscription<Action<Entity>>(
                h => World.Instance.TurnManager.OnTurnStart += h,
                h => World.Instance.TurnManager.OnTurnStart -= h,
                OnTurnStart
            );
        }

        private void OnTurnStart(Entity turnTaker)
        {
            if (turnTaker == World.Instance.GetPlayerOf(Owner))
            {
                if (Remaining <= 0)
                    World.Instance.RemoveEntity(Owner);
                else
                    Remaining--;
            }
        }
    }
}
