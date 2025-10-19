namespace CBA
{
    public class GetsRandomItems(Entity owner) : Component(owner)
    {
        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Player)
                throw new InvalidOperationException($"{Owner.Id} was given an invalid Component: GetRandomItems.");
        }
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += player => { if (player == Owner) GiveRandomItem(); };
        }
        private void GiveRandomItem()
        {
            ItemFactory.CreateRandomItem(Owner);
            if (Random.Shared.NextDouble() < Owner.GetComponent<StatsComponent>().GetHyperbolic("Luck"))
                ItemFactory.CreateRandomItem(Owner);
        }
    }
}
