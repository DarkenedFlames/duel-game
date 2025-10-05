using System;

namespace MyApp
{
    public class World
    {
        private readonly List<Entity> _entities = new();

        public Entity CreateEntity()
        {
            var entity = new Entity();
            _entities.Add(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
        }

        public IEnumerable<Entity> Query<T1>() where T1 : Component
        {
            return _entities.Where(e => e.HasComponent<T1>());
        }

        public IEnumerable<Entity> Query<T1, T2>()
            where T1 : Component
            where T2 : Component
        {
            return _entities.Where(e => e.HasComponent<T1>() && e.HasComponent<T2>());
        }
    }
}