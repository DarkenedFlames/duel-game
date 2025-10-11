namespace CBA
{
    public class EffectDuration(Entity owner, int maxDuration) : Component(owner)
    {
        public int Remaining { get; set; } = maxDuration;
        public int Maximum { get; set; } = maxDuration;

        public override void Subscribe()
        {
            Owner.GetComponent<EffectData>()?.PlayerEntity?.GetComponent<TakesTurns>()?.OnTurnStart += _ => TickDuration();
        }

        public void TickDuration()
        {
            if (Remaining <= 0)
            {
                World.Instance.RemoveEntity(Owner);
            }
            else
            {
                Remaining--;
            }
        }

    }
}
