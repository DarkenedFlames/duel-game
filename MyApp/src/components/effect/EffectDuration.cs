namespace CBA
{
    public class EffectDuration(Entity owner, int maxDuration) : Component(owner)
    {
        public int Remaining { get; set; } = maxDuration;
        public int Maximum { get; set; } = maxDuration;

        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += turnTaker =>
            {
                if (turnTaker == World.Instance.GetPlayerOf(Owner)) TickDuration();
            };
        }
        public void TickDuration()
        {
            if (Remaining <= 0) World.Instance.RemoveEntity(Owner);
            else Remaining--;
        }
    }
}
