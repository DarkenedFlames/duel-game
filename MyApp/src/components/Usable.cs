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
            ItemData itemData = Helper.ThisIsNotNull(
                Owner.GetComponent<ItemData>(),
                "Useable.TryUse: Unexpected null value for itemData."
            );

            Entity user = Helper.ThisIsNotNull(
                itemData.PlayerEntity,
                "Usable.TryUse: Unexpected null value for user."
            );

            ResourcesComponent resources = Helper.ThisIsNotNull(
                user.GetComponent<ResourcesComponent>(),
                "Useable.TryUse: Unexpected null value for resources."
            );


            if (resources.Get("Stamina") < StaminaCost)
            {
                OnUseFailed?.Invoke(Owner, target);
                return;
            }

            resources.Change("Stamina", -StaminaCost);
            OnUseSuccess?.Invoke(Owner, target);
        }
    }
}
