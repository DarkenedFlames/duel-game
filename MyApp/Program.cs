using CBA;

public class Program
{
    static void Main()
    {
        World world = new();
        List<string> playerNames = ["Bob", "Alice"];

        foreach (string? name in playerNames)
        {
            Entity player = new PlayerEntity();
            new PlayerData(player, name);
            new StatsComponent(player);
            new ResourcesComponent(player);
            new GetsRandomItems(player);
            new PeersComponent(player);
            new RefillsStamina(player);

            player.SubscribeAll();
            World.Instance.AddEntity(player);
        }
        world.TurnManager.StartGameLoop();
    }
}


namespace CBA
{
    public class PlayerEntity : Entity { }
}
