namespace CBA
{
    public class RefillsStamina(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += (player) =>
            {
                if (player == Owner)
                {
                    RefillStamina(player);
                }
            };
        }

        private void RefillStamina(Entity owner)
        {
            Owner.GetComponent<ResourcesComponent>()?.Refill("Stamina");
        }
    }

}