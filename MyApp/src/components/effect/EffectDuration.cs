namespace CBA
{
    public class EffectDuration(Entity owner, int maxDuration) : Component(owner)
    {
        public int Remaining { get; set; } = maxDuration;
        public int Maximum { get; set; } = maxDuration;

        protected override void RegisterSubscriptions()
        {
            RegisterSubscription<Action<Entity>>(
                h => World.Instance.TurnManager.OnTurnEnd += h,
                h => World.Instance.TurnManager.OnTurnEnd -= h,
                OnTurnEnd
            );
        }
        private void OnTurnEnd(Entity turnTaker)
        {
            if (turnTaker == World.GetPlayerOf(Owner))
            {
                if (Remaining <= 1)
                    World.Instance.RemoveEntity(Owner);
                else
                    Remaining--;
            }
        }
    }
}
