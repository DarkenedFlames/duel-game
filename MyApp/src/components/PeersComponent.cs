namespace CBA
{
    public class PeersComponent(Entity owner) : Component(owner)
    {
        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Player)
                throw new InvalidOperationException($"PeersComponent was given to an invalid category of entity: {Owner.Id}.");
        }
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += (player) => { if (player == Owner) RevealInventory(); };
        }
        private void RevealInventory()
        {
            StatsComponent stats = Owner.GetComponent<StatsComponent>();

            float chance = stats.GetHyperbolic("Peer");
            if (chance <= 0) return;

            if (Random.Shared.NextDouble() >= chance) return;

            // --- Get all other players ---
            List<Entity> enemies = [..World.Instance.GetById(EntityCategory.Player).Where(p => p != Owner)];
            if (enemies.Count == 0) return;

            // --- Pick one random enemy ---
            Entity enemy = enemies[Random.Shared.Next(enemies.Count)];

            // --- Find all items belonging to that enemy ---
            List<string> enemyItemNames = [..World.Instance
                .GetById(EntityCategory.Item)
                .Where(e => e.GetComponent<ItemData>().PlayerEntity == enemy)
                .Select(e => e.DisplayName)];

            if (enemyItemNames.Count > 0) Printer.PrintPeered(enemyItemNames, enemy);
        }
    }
}
