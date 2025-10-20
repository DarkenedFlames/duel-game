namespace CBA
{
    public class RefillsStamina(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += turnTaker =>
            {
                if (turnTaker == Owner) RefillStamina(); 
            };
        }
        private void RefillStamina() => Owner.GetComponent<ResourcesComponent>().Refill("Stamina");
    }
}