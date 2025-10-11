namespace CBA
{
    public class Usable(Entity owner, int staminaCost) : Component(owner)
    {
        public int StaminaCost { get; init; } = staminaCost;

        public event Action<Entity, Entity>? OnUseSuccess;
        public event Action<Entity, Entity>? OnUseFailed;

        public override void Subscribe()
        {
            // Could subscribe to external triggers if needed
        }

        public void TryUse(Entity target)
        {
            var itemData = Owner.GetComponent<ItemData>() ?? throw new InvalidOperationException("ItemData component missing");
            var user = itemData.PlayerEntity;

            var resources = user.GetComponent<ResourcesComponent>();
            if (resources == null || resources.Get("Stamina") < StaminaCost)
            {
                OnUseFailed?.Invoke(Owner, target);
                return;
            }

            resources.Change("Stamina", -StaminaCost);
            OnUseSuccess?.Invoke(Owner, target);
        }
    }
}
