using System;

namespace CBA
{
    public abstract class Entity
    {
        private readonly List<Component> _components = new();

        public Entity()
        {
            World.Instance.AddEntity(this);
        }

        public void AddComponent<T>(T component) where T : Component
        {
            _components.Add(component);
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