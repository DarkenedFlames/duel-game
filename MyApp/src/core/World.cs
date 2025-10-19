namespace CBA
{
    public enum QueryReturnType
    {
        Object,
        Data,
        Name
    }
    public class World
    {
        public static World Instance { get; private set; } = null!;

        public TurnManager TurnManager { get; private set; }
        private readonly List<Entity> _entities = [];
        private readonly Dictionary<(EntityCategory, string), int> _instanceCounters = [];

        public event Action<Entity>? OnEntityAdded;
        public event Action<Entity>? OnEntityRemoved;

        public World()
        {
            if (Instance != null)
                throw new InvalidOperationException("Only one World instance allowed!");

            Instance = this;
            TurnManager = new TurnManager();

            OnEntityAdded += Printer.PrintEntityAdded;
            OnEntityRemoved += Printer.PrintEntityRemoved;
        }

        // ========== ENTITY MANAGEMENT ==========
        public void AddEntity(Entity entity)
        {
            if (_entities.Contains(entity))
                throw new InvalidOperationException($"World.AddEntity: Entity, {entity}, already exists in the world.");
            if (entity.Id.Category == EntityCategory.Player && !entity.HasComponent<PlayerData>())
                throw new InvalidOperationException($"World.AddEntity: Entity of Category EntityCategory.Player added with no PlayerData Component.");
            if (entity.Id.Category == EntityCategory.Item && !entity.HasComponent<ItemData>())
                throw new InvalidOperationException($"World.AddEntity: Entity of Category EntityCategory.Item added with no ItemData Component.");
            if (entity.Id.Category == EntityCategory.Effect && !entity.HasComponent<EffectData>())
                throw new InvalidOperationException($"World.AddEntity: Entity of Category EntityCategory.Effect added with no EffectData Component.");

            _entities.Add(entity);
            entity.ValidateAll();
            entity.SubscribeAll();
            OnEntityAdded?.Invoke(entity);
        }
        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
            OnEntityRemoved?.Invoke(entity);
        }
        public IEnumerable<Entity> GetById(EntityCategory? category = null, string? typeId = null, int? instanceId = null)
        {
            return _entities.Where(e =>
                (category == null || e.Id.Category == category) &&
                (typeId == null || e.Id.TypeId == typeId) &&
                (instanceId == null || e.Id.InstanceId == instanceId)
            );
        }
        public int GenerateInstanceId(EntityCategory category, string typeId)
                {
                    (EntityCategory, string) key = (category, typeId);
                    if (!_instanceCounters.ContainsKey(key)) _instanceCounters[key] = 0;
                    return ++_instanceCounters[key];
                }
        public IEnumerable<Entity> GetEntitiesWith<T>() where T : Component =>
            _entities.Where(e => e.HasComponent<T>());
        public IEnumerable<T> GetAllForPlayer<T>(Entity player, EntityCategory category, string? typeId = null, bool? equipped = null)
        {
            // Step 1: Start from all entities in this category (optionally narrowed by typeId)
            var entities = GetById(category, typeId);

            // Step 2: Filter for entities owned by this player
            entities = category switch
            {
                EntityCategory.Item => entities.Where(e => e.GetComponent<ItemData>().PlayerEntity == player),
                EntityCategory.Effect => entities.Where(e => e.GetComponent<EffectData>().PlayerEntity == player),
                _ => throw new ArgumentException($"Unsupported category: {category}")
            };

            // Step 3: Filter by equipment status if requested
            if (category == EntityCategory.Item && equipped.HasValue)
            {
                if (equipped.Value)
                {
                    // Only equipped items
                    entities = entities.Where(e =>
                        e.HasComponent<Wearable>() && e.GetComponent<Wearable>().IsEquipped);
                }
                else
                {
                    // Non-equipped items or items without wearable component
                    entities = entities.Where(e =>
                        !e.HasComponent<Wearable>() || !e.GetComponent<Wearable>().IsEquipped);
                }
            }

            // Step 4: Determine output type
            if (typeof(T) == typeof(Entity))
                return entities.Cast<T>();

            if (typeof(T) == typeof(ItemData))
                return entities.Select(e => e.GetComponent<ItemData>()).Cast<T>();

            if (typeof(T) == typeof(EffectData))
                return entities.Select(e => e.GetComponent<EffectData>()).Cast<T>();

            if (typeof(T) == typeof(string))
                return entities.Select(e => e.DisplayName).Cast<T>();

            throw new InvalidOperationException($"Unsupported return type: {typeof(T).Name}");
        }
        public IEnumerable<Entity> GetAllPlayers(Entity? excludePlayer = null)
        {
            var players = GetById(EntityCategory.Player);

            players = players.Where(e =>
            {
                var resources = e.GetComponent<ResourcesComponent>();
                var health = resources.Get("Health");
                return health > 0 && (excludePlayer == null || e != excludePlayer);
            });

            return players;
        }
        public Entity GetPlayerOf(Entity entity)
        {
            switch (entity.Id.Category)
            {
                case EntityCategory.Item: return entity.GetComponent<ItemData>().PlayerEntity;
                case EntityCategory.Effect: return entity.GetComponent<EffectData>().PlayerEntity;
                default: throw new InvalidOperationException($"Invalid category for GetPlayerOf: {entity.Id.Category}");
            }
        }
    }
}
