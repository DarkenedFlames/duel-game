namespace CBA
{
    public class Entity(EntityCategory category, string typeId, string displayName)
    {
        private readonly List<Component> _components = [];
        public string DisplayName { get; } = displayName;
        public EntityId Id { get; } = new EntityId(category, typeId, World.Instance.GenerateInstanceId(category, typeId));

        public void AddComponent<T>(T component) where T : Component
        {
            _components.Add(component);
        }

        public T GetComponent<T>() where T : Component
        {
            T? c = _components.OfType<T>().FirstOrDefault() ?? throw new InvalidOperationException($"[{Id}] Missing required component: {typeof(T).Name}");
            return c;
        }

        public bool HasComponent<T>() where T : Component => _components.OfType<T>().Any();

        public void RemoveComponent<T>() where T : Component
        {
            T? c = _components.OfType<T>().FirstOrDefault();
            if (c != null) _components.Remove(c);
        }

        public IEnumerable<Component> GetAllComponents() => _components;

        public void SubscribeAll()
        {
            foreach (Component component in _components)
                component.Subscribe();
            
        }

        public void ValidateAll()
        {
            foreach (Component component in _components)
                component.ValidateDependencies();
        }
    }
}
