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
            Entity user = World.Instance.GetPlayerOf(Owner);
            ResourcesComponent resources = user.GetComponent<ResourcesComponent>();
            StatsComponent stats = user.GetComponent<StatsComponent>();
            ItemType type = Owner.GetComponent<ItemData>().Type;

            float multiplier;
            if (type == ItemType.Consumable)
                multiplier = 1 - stats.GetHyperbolic("ConsumableCost");
            else
                multiplier = 1 - stats.GetHyperbolic("WeaponCost");

            if (resources.Get("Stamina") < StaminaCost * multiplier)
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
