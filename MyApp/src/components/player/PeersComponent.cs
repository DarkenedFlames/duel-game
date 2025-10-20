namespace CBA
{
    public class PeersComponent(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += turnTaker =>
            {
                if (turnTaker == Owner) RevealInventory();
            };
        }
        private void RevealInventory()
        {
            StatsComponent stats = Owner.GetComponent<StatsComponent>();

            float chance = stats.GetHyperbolic("Peer");
            if (chance <= 0) return;

            if (Random.Shared.NextDouble() >= chance) return;

            List<Entity> enemies = [..World.Instance.GetAllPlayers(Owner)];
            if (enemies.Count == 0) return;
            
            Entity enemy = enemies[Random.Shared.Next(enemies.Count)];
            List<string> enemyItemNames = [.. World.Instance.GetAllForPlayer<string>(enemy, EntityCategory.Item)];

            if (enemyItemNames.Count > 0) Printer.PrintPeered(enemyItemNames, enemy);
        }
    }
}
