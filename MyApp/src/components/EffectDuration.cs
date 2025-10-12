namespace CBA
{
    public class EffectDuration(Entity owner, int maxDuration) : Component(owner)
    {
        public int Remaining { get; set; } = maxDuration;
        public int Maximum { get; set; } = maxDuration;

        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += (player) =>
            {
                if (player == Owner.GetComponent<EffectData>()?.PlayerEntity)
                {
                    TickDuration();
                }
            };
            
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
