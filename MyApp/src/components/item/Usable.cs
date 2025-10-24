namespace CBA
{
    public class Usable(Entity owner, int staminaCost) : Component(owner)
    {
        private int StaminaCost { get; init; } = staminaCost;

        public event Action<Entity, Entity>? OnUseSuccess;
        public event Action<Entity, Entity>? OnUseFailed;

        protected override void RegisterSubscriptions() { }

        public void TryUse(Entity target)
        {
            Entity user = World.GetPlayerOf(Owner);
            ResourcesComponent resources = user.GetComponent<ResourcesComponent>();
            StatsComponent stats = user.GetComponent<StatsComponent>();
            ItemData itemData = Owner.GetComponent<ItemData>();

            float floatCost = StaminaCost;
            float multiplier = itemData.Type switch
            {
                ItemType.Consumable => stats.GetLinearClamped("ConsumableCost", .25f),
                ItemType.Weapon     => stats.GetLinearClamped("WeaponCost", .25f),
                _                   => 1f
            };

            floatCost *= multiplier;

            if (resources.Get("Stamina") < floatCost)
            {
                Printer.PrintInsufficientStamina(Owner);
                OnUseFailed?.Invoke(Owner, target);
                return;
            }

            int finalCost = (int)floatCost;

            resources.Change("Stamina", -finalCost);
            Printer.PrintItemUsed(Owner, target);
            OnUseSuccess?.Invoke(Owner, target);

            if (itemData.Type == ItemType.Consumable)
            {
                Printer.PrintItemConsumed(Owner);
                World.Instance.RemoveEntity(Owner);
            }
        }
    }
}
