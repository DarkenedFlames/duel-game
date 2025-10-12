using CBA;

public class Program
{
    static void Main()
    {
        World world = new();
        var playerNames = new List<string> { "Bob", "Alice" };

        foreach (var name in playerNames)
        {
            var player = new PlayerEntity();
            new PlayerData(player, name);
            new StatsComponent(player);
            new ResourcesComponent(player);
            new GetsRandomItems(player);
            new PeersComponent(player);
            new RefillsStamina(player);

            player.SubscribeAll();
        }
        world.TurnManager.StartGameLoop();
    }
}


namespace CBA
{
    public class PlayerEntity : Entity { }
}
