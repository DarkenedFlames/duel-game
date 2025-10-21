using CBA;

public class Program
{
    static void Main()
    {
        World world = new();
        
        List<Entity> players =
        [
            new(EntityCategory.Player, "Bob", "Bob"),
            new(EntityCategory.Player, "Alice", "Alice")
        ];

        foreach (Entity p in players)
        {
            List<Component> Components =
            [
                new PlayerData(p),
                new StatsComponent(p),
                new ResourcesComponent(p),
                new GetsRandomItems(p),
                new PeersComponent(p),
                new RefillsStamina(p),
                new TurnMemory(p)
            ];

            foreach (Component component in Components)
                p.AddComponent(component);


            World.Instance.AddEntity(p);
        }

        world.TurnManager.StartGameLoop();
    }
}