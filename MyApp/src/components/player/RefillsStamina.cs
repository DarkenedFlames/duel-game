namespace CBA
{
    public class RefillsStamina(Entity owner) : Component(owner)
    {
        protected override void RegisterSubscriptions()
        {
            RegisterSubscription<Action<Entity>>(
                h => World.Instance.TurnManager.OnTurnStart += h,
                h => World.Instance.TurnManager.OnTurnStart -= h,
                turnTaker => { if (turnTaker == Owner) RefillStamina(); }
            );
        }
        private void RefillStamina() => Owner.GetComponent<ResourcesComponent>().Refill("Stamina");
    }
}