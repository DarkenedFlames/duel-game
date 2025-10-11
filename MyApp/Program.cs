using CBA;
using System;
using System.Collections.Generic;

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
            var stats = new StatsComponent(player); // see if stats need to be passed into resources
            player.AddComponent(stats);
            player.AddComponent(new ResourcesComponent(player, stats));
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
