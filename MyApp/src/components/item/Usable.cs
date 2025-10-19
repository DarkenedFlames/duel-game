namespace CBA
{
    public class Usable(Entity owner, int staminaCost) : Component(owner)
    {
        public int StaminaCost { get; init; } = staminaCost;

        public event Action<Entity, Entity>? OnUseSuccess;
        public event Action<Entity, Entity>? OnUseFailed;

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Item)
                throw new InvalidOperationException($"Usable was given to an invalid category of entity: {Owner.Id}.");
        }
        public override void Subscribe()
        {
            OnUseSuccess += Printer.PrintItemUsed;
            OnUseFailed += (_, _) => Printer.PrintInsufficientStamina(Owner);
        }

        public void TryUse(Entity target)
        {
            if (target.Id.Category != EntityCategory.Player)
                throw new InvalidOperationException($"[{Owner.Id}] Usable.TryUse was passed a non-player target.");
            
            ResourcesComponent resources = World.Instance.GetPlayerOf(Owner).GetComponent<ResourcesComponent>();
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
