namespace CBA
{
    public abstract class Entity
    {
        private readonly List<Component> _components = new();

        public Entity()
        {
            //World.Instance.AddEntity(this);
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
            T? c = _components.OfType<T>().FirstOrDefault();
            if (c != null) _components.Remove(c);
        }

        // === New Methods ===

        /// <summary>
        /// Returns all components of this entity.
        /// </summary>
        public IEnumerable<Component> GetAllComponents() => _components;

        /// <summary>
        /// Calls Subscribe() on all components.
        /// </summary>
        public void SubscribeAll()
        {
            foreach (Component? component in _components)
            {
                component.Subscribe();
            }
        }
    }
}
