namespace CBA
{
    public class EffectDuration(Entity owner, int maxDuration) : Component(owner)
    {
        public int Remaining { get; set; } = maxDuration;
        public int Maximum { get; set; } = maxDuration;

        public override void Subscribe()
        {
            EffectData effectData = Helper.ThisIsNotNull(Owner.GetComponent<EffectData>());

            World.Instance.TurnManager.OnTurnStart += (player) =>
            {
                if (player == effectData.PlayerEntity) TickDuration();
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
