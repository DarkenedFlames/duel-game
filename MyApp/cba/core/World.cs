using System;

namespace CBA
{
    public class World
    {
        // Static reference to the "singleton" world
        public static World Instance { get; private set; } = null!;

        public TurnManager TurnManager { get; private set; }
        private readonly List<Entity> _entities = new();

        public event Action<Entity>? OnEntityAdded;
        public event Action<Entity>? OnEntityRemoved;

        public World()
        {
            if (Instance != null)
                throw new InvalidOperationException("Only one World instance allowed!");
            
            Instance = this;  // set the static reference
            TurnManager = new TurnManager();

            OnEntityAdded += entity => Printer.PrintEntityAdded(entity);
            OnEntityRemoved += entity => Printer.PrintEntityRemoved(entity);
        }

        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
            OnEntityAdded?.Invoke(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
            OnEntityRemoved?.Invoke(entity);
        }

        public IEnumerable<T> GetEntitiesOfType<T>() where T : Entity =>
            _entities.OfType<T>();

        public IEnumerable<Entity> GetEntitiesWith<T>() where T : Component =>
            _entities.Where(e => e.HasComponent<T>());
    }

}