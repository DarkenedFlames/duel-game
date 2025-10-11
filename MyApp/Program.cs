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
            player.AddComponent(new PlayerData(player, name));
            player.AddComponent(new StatsComponent(player));
            player.AddComponent(new ResourcesComponent(player));
            player.AddComponent(new TakesTurns(player)); 
            player.AddComponent(new GetsRandomItems(player));
            player.AddComponent(new PeersComponent(player));
            player.AddComponent(new RefillsStamina(player));

            player.SubscribeAll();
        }
        world.TurnManager.StartGameLoop();
    }
}


namespace CBA
{
    public class PlayerEntity : Entity { }
}
