namespace CBA
{
    public class PeersComponent(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += (player) =>
            {
                Helper.ThisIsNotNull(player, "PeersComponent: Player entity in OnTurnStart was null.");
                if (player == Owner) RevealInventory(player);
            };
        }

        private void RevealInventory(Entity playerEntity)
        {
            StatsComponent stats = Helper.ThisIsNotNull(
                playerEntity.GetComponent<StatsComponent>(),
                "PeersComponent.RevealInventory: StatsComponent was missing from playerEntity."
            );

            float chance = stats.GetHyperbolic("Peer");
            if (chance <= 0) return;

            if (Random.Shared.NextDouble() >= chance) return;

            // --- Get all other players ---
            List<Entity> allPlayers = World.Instance.GetAllPlayers().ToList();
            Helper.ThisIsNotNull(allPlayers, "PeersComponent.RevealInventory: GetAllPlayers returned null or empty list.");

            List<Entity> enemies = allPlayers.Where(p => p != playerEntity).ToList();
            if (enemies.Count == 0) return;

            // --- Pick one random enemy ---
            Entity enemy = enemies[Random.Shared.Next(enemies.Count)];
            Helper.ThisIsNotNull(enemy, "PeersComponent.RevealInventory: Selected enemy was null.");

            // --- Find all items belonging to that enemy ---
            List<string> enemyItems = [.. World.Instance.GetItemsForPlayer(enemy)
                .Select(i => i.GetComponent<ItemData>()?.Name ?? "Unknown")];

            if (enemyItems.Count > 0) Printer.PrintPeered(enemyItems, enemy);
        }
    }
}
