using System;
using System.Collections.Generic;
using System.Linq;

namespace CBA
{
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

        public IEnumerable<Entity> GetById
        (
            EntityCategory? category = null,
            string? typeId = null,
            int? instanceId = null
        )
        {
            return _entities.Where(e =>
                (category == null || e.Id.Category == category) &&
                (typeId == null || e.Id.TypeId == typeId) &&
                (instanceId == null || e.Id.InstanceId == instanceId)
            );
        }


        // ========== GENERIC HELPERS ==========
        public IEnumerable<Entity> GetEntitiesWith<T>() where T : Component =>
            _entities.Where(e => e.HasComponent<T>());

        // ========== SPECIFIC HELPERS ==========

        public IEnumerable<Entity> GetAllPlayers() =>
            _entities.Where(e => e.HasComponent<PlayerData>());

        public IEnumerable<Entity> GetAllItems() =>
            _entities.Where(e => e.HasComponent<ItemData>());

        public IEnumerable<Entity> GetAllEffects() =>
            _entities.Where(e => e.HasComponent<EffectData>());

        // ========== OWNERSHIP HELPERS ==========

        public IEnumerable<Entity> GetItemsForPlayer(Entity player) =>
            _entities
                .Where(e => e.HasComponent<ItemData>())
                .Where(e => e.GetComponent<ItemData>()?.PlayerEntity == player);

        public IEnumerable<Entity> GetEffectsForPlayer(Entity player) =>
            _entities
                .Where(e => e.HasComponent<EffectData>())
                .Where(e => e.GetComponent<EffectData>()?.PlayerEntity == player);

        // (Optional) Get all entities owned by a player (items, effects, summons, etc.)
        public IEnumerable<Entity> GetAllOwnedBy(Entity player) =>
            _entities.Where(e =>
                (e.GetComponent<ItemData>()?.PlayerEntity == player) ||
                (e.GetComponent<EffectData>()?.PlayerEntity == player));


        public Entity? GetPlayerOfItem(Entity item)
        {
            ItemData? itemData = item.GetComponent<ItemData>();
            return itemData?.PlayerEntity;
        }

        public Entity? GetPlayerOfEffect(Entity effect)
        {
            EffectData? effectData = effect.GetComponent<EffectData>();
            return effectData?.PlayerEntity;
        }

        public Entity? GetPlayerOf(Entity entity)
        {
            // Check if it’s an item
            ItemData? itemData = entity.GetComponent<ItemData>();
            if (itemData?.PlayerEntity != null)
                return itemData.PlayerEntity;

            // Check if it’s an effect
            EffectData? effectData = entity.GetComponent<EffectData>();
            if (effectData?.PlayerEntity != null)
                return effectData.PlayerEntity;

            // Not an item or effect, or no owner assigned
            return null;
        }

        public int GenerateInstanceId(EntityCategory category, string typeId)
        {
            (EntityCategory, string) key = (category, typeId);
            if (!_instanceCounters.ContainsKey(key)) _instanceCounters[key] = 0;
            return ++_instanceCounters[key];
        }


    }
}
