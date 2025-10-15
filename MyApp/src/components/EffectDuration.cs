namespace CBA
{
    public class EffectDuration(Entity owner, int maxDuration) : Component(owner)
    {
        public int Remaining { get; set; } = maxDuration;
        public int Maximum { get; set; } = maxDuration;

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Effect)
                throw new InvalidOperationException($"{Owner.Id} was given an invalid Component: EffectDuration.");
        }
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += player =>
            {
                if (player == Owner.GetComponent<EffectData>().PlayerEntity) TickDuration();
            };

        }
        public void TickDuration()
        {
            if (Remaining <= 0)
                World.Instance.RemoveEntity(Owner);
            else
                Remaining--;
        }
    }
}
