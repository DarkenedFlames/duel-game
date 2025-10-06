using System;

namespace MyCBA
{
    public class Entity
    {
        private readonly Dictionary<Type, Component> _components = new();
        public string Name { get; set; } = "Unnamed Entity";

        public T? GetComponent<T>() where T : Component
        {
            _components.TryGetValue(typeof(T), out var comp);
            return comp as T;
        }

        public void AddComponent(Component component)
        {
            _components[component.GetType()] = component;
            component.Owner = this;
        }

        public bool HasComponent<T>() where T : Component => _components.ContainsKey(typeof(T));
    }
}