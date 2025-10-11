namespace CBA
{
    public class PeersComponent(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            Owner.GetComponent<TakesTurns>()?.OnTurnStart += RevealInventory;
        }

        private void RevealInventory(Entity playerEntity)
        {
            if (playerEntity == null) return;

            var stats = playerEntity.GetComponent<StatsComponent>();
            var chance = stats?.GetHyperbolic("Peer");
            if (chance <= 0) return;

            if (Random.Shared.NextDouble() >= chance) return;

            // --- Get all other players ---
            var allPlayers = World.Instance.GetEntitiesWith<PlayerData>().ToList();
            var enemies = allPlayers.Where(p => p != playerEntity).ToList();
            if (enemies.Count == 0) return;

            // --- Pick one random enemy ---
            var enemy = enemies[Random.Shared.Next(enemies.Count)];

            // --- Find all items belonging to that enemy ---
            var enemyItems = World.Instance.GetEntitiesWith<ItemData>()
                .Where(i => i.GetComponent<ItemData>()?.PlayerEntity == enemy)
                .Select(i => i.GetComponent<ItemData>()?.Name ?? "Unknown")
                .ToList();

            if (enemyItems.Count > 0)
            {
                Printer.PrintPeered(enemyItems, enemy);
            }
        }
    }
}
