namespace CBA
{
    public class GetsRandomItems(Entity owner) : Component(owner)
    {
        public bool DebugModeEnabled = true; // Toggle this on for testing
        public Queue<string> DebugItemQueue = new(
        [
            "brandish",
            "corona"
        ]);
        protected override void RegisterSubscriptions()
        {
            RegisterSubscription<Action<Entity>>(
                h => World.Instance.TurnManager.OnTurnStart += h,
                h => World.Instance.TurnManager.OnTurnStart -= h,
                turnTaker => { if (turnTaker == Owner) GiveItem(); }
            );
        }
        private void GiveItem()
        {
            if (DebugModeEnabled && DebugItemQueue.Count > 0)
            {
                string nextItemId = DebugItemQueue.Dequeue();
                ItemFactory.CreateSpecificItem(Owner, nextItemId);
                return;
            }

            ItemFactory.CreateRandomItem(Owner);

            float luck = Owner.GetComponent<StatsComponent>().GetHyperbolic("Luck");
            if (Random.Shared.NextDouble() < luck)
                ItemFactory.CreateRandomItem(Owner);
        }
    }
}