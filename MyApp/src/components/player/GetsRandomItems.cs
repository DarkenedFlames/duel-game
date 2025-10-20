namespace CBA
{
    public class GetsRandomItems(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += turnTaker =>
            { 
                if (turnTaker == Owner) GiveRandomItem(); 
            };
        }
        private void GiveRandomItem()
        {
            ItemFactory.CreateRandomItem(Owner);
            if (Random.Shared.NextDouble() < Owner.GetComponent<StatsComponent>().GetHyperbolic("Luck"))
                ItemFactory.CreateRandomItem(Owner);
        }
    }
}
