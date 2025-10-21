namespace CBA
{
    public class GetsRandomItems(Entity owner) : Component(owner)
    {
        protected override void RegisterSubscriptions()
        {
            RegisterSubscription<Action<Entity>>(
                h => World.Instance.TurnManager.OnTurnStart += h,
                h => World.Instance.TurnManager.OnTurnStart -= h,
                turnTaker => { if (turnTaker == Owner) GiveRandomItem(); }
            );
        }
        private void GiveRandomItem()
        {
            ItemFactory.CreateRandomItem(Owner);
            if (Random.Shared.NextDouble() < Owner.GetComponent<StatsComponent>().GetHyperbolic("Luck"))
                ItemFactory.CreateRandomItem(Owner);
        }
    }
}
