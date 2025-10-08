using System;

namespace CBA
{
    public abstract class Entity
    {
        public World World { get; }
        private readonly List<Component> _components = new();

        public Entity(World world)
        {
            World = world;
            world.AddEntity(this);
        }

        public T AddComponent<T>(T component) where T : Component
        {
            _components.Add(component);
            component.Initialize(this);
            return component;
        }

        public T? GetComponent<T>() where T : Component =>
            _components.OfType<T>().FirstOrDefault();

        public bool HasComponent<T>() where T : Component =>
            _components.OfType<T>().Any();

        public void RemoveComponent<T>() where T : Component
        {
            var c = _components.OfType<T>().FirstOrDefault();
            if (c != null) _components.Remove(c);
        }
    }
}