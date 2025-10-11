namespace CBA
{
    public class RefillsStamina(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            Owner.GetComponent<TakesTurns>()?.OnTurnStart += RefillStamina;
        }

        private void RefillStamina(Entity owner)
        {
            Owner.GetComponent<ResourcesComponent>()?.Refill("Stamina");
        }
    }

}