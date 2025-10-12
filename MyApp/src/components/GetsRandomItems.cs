namespace CBA
{
    public class GetsRandomItems(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += (player) =>
            {
                if (player == Owner)
                {
                    GiveRandomItem(player);
                }
            };
        }

        private void GiveRandomItem(Entity player)
        {
            if (player == null) return;
            ItemFactory.CreateRandomItem(player);
            if (Random.Shared.NextDouble() < player.GetComponent<StatsComponent>()?.GetHyperbolic("Luck"))
                ItemFactory.CreateRandomItem(player);
        }
    }
}
