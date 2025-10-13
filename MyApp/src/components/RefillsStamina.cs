namespace CBA
{
    public class RefillsStamina(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += (player) =>
            {
                if (player == Owner) RefillStamina();
            };
        }

        private void RefillStamina()
        {
            ResourcesComponent playerResources = Helper.ThisIsNotNull(
                Owner.GetComponent<ResourcesComponent>(),
                "RefillsStamina.RefillStamina: Unexpected null value for player's ResourcesComponent."
            );
            
            playerResources.Refill("Stamina");
        }
    }

}