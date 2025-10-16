using CBA;

public class Program
{
    static void Main()
    {
        World world = new();
        Entity bob = new(EntityCategory.Player, "Bob", "Bob");
        Entity alice = new(EntityCategory.Player, "Alice", "Alice");
        List<Entity> players = [bob, alice];

        foreach (Entity player in players)
        {
            new PlayerData(player, player.DisplayName);
            new StatsComponent(player);
            new ResourcesComponent(player);
            new GetsRandomItems(player);
            new PeersComponent(player);
            new RefillsStamina(player);

            World.Instance.AddEntity(player);
        }
        world.TurnManager.StartGameLoop();
    }
}
