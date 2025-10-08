using System;

namespace CBA
{
    public class World
    {
        private readonly List<Entity> _entities = new();

        public event Action<Entity>? OnEntityAdded;
        public event Action<Entity>? OnEntityRemoved;

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