namespace CBA
{
    public class RefillsStamina(Entity owner) : Component(owner)
    {
        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Player)
                    throw new InvalidOperationException($"RefillsStamina was given to an invalid category of entity: {Owner.Id}.");
        }
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += player => { if (player == Owner) RefillStamina(); };
        }
        private void RefillStamina() => Owner.GetComponent<ResourcesComponent>().Refill("Stamina");
    }

}