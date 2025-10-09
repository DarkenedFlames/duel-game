using System;

namespace CBA
{
    public class Usable : Component
    {
        public int StaminaCost { get; init; } = 0;

        public event Action<Entity, Entity>? OnUseSuccess;
        public event Action<Entity, Entity>? OnUseFailed;
        public Usable(Entity owner, int staminaCost) : base(owner)
        {
            StaminaCost = staminaCost;
        }

        protected override void Subscribe()
        {
            // Could subscribe to external triggers if needed
        }

        public void TryUse(Entity target)
        {
            var itemData = Owner.GetComponent<ItemData>();
            if (itemData == null)
                throw new InvalidOperationException("ItemData component missing");

            var user = itemData.PlayerEntity;

            var resources = user.GetComponent<ResourcesComponent>();
            if (resources == null || resources.Get("Stamina") < StaminaCost)
            {
                OnUseFailed?.Invoke(user, target);
                return;
            }

            resources.Change("Stamina", -StaminaCost);
            OnUseSuccess?.Invoke(user, target);
        }
    }
}
