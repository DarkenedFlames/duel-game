namespace CBA
{
    public class Usable(Entity owner, int staminaCost) : Component(owner)
    {
        public int StaminaCost { get; init; } = staminaCost;

        public event Action<Entity, Entity>? OnUseSuccess;
        public event Action<Entity, Entity>? OnUseFailed;

        public override void Subscribe(){}

        public void TryUse(Entity target)
        {            
            ResourcesComponent resources = World.Instance.GetPlayerOf(Owner).GetComponent<ResourcesComponent>();
            if (resources.Get("Stamina") < StaminaCost)
            {
                Printer.PrintInsufficientStamina(Owner);
                OnUseFailed?.Invoke(Owner, target);
                return;
            }

            resources.Change("Stamina", -StaminaCost);
            Printer.PrintItemUsed(Owner, target);
            OnUseSuccess?.Invoke(Owner, target);
        }
    }
}
