namespace CBA
{
    public class GetsRandomItems(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += (player) =>
            {
                if (player == Owner) GiveRandomItem(player);
            };
        }

        private void GiveRandomItem(Entity player)
        {
            // Assert that the player must have stats.
            StatsComponent playerStats = Helper.ThisIsNotNull
            (
                player.GetComponent<StatsComponent>(),
                "GetsRandomItems.GiveRandomItem: Unexpected null value for player's StatsComponent."
            );

            // Give item to player; give a second item based on their luck.
            ItemFactory.CreateRandomItem(player);
            if (Random.Shared.NextDouble() < playerStats.GetHyperbolic("Luck"))
                ItemFactory.CreateRandomItem(player);
        }
    }
}
