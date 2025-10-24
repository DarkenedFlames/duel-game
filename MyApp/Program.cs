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
            p.AddComponents<Component>
            (
                [
                    new PlayerData(p),
                    new StatsComponent(p),
                    new ResourcesComponent(p),
                    new GetsRandomItems(p),
                    new RefillsStamina(p),
                    new TurnMemory(p)
                ]
            );

            World.Instance.AddEntity(p);
        }

        world.TurnManager.StartGameLoop();
    }
}