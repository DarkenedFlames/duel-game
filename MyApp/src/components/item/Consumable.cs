namespace CBA
{
    public class Consumable(Entity owner) : Component(owner)
    {
        public event Action<Entity>? OnConsumed;

        public override void ValidateDependencies()
        {
            // Validate Owner Category
            if (Owner.Id.Category != EntityCategory.Item)
                throw new InvalidOperationException($"{Owner.Id} was given an incompatible Component: Consumable.");
            
            // Validate Component Dependencies
            if (!Owner.HasComponent<Usable>())
                throw new InvalidOperationException($"Component Missing a Dependency: (Owner: {Owner.Id}, Component: Consumable, Dependency: Usable.");
        }
        public override void Subscribe()
        {
            Owner.GetComponent<Usable>().OnUseSuccess += (_, _) => OnUseSuccess();
        }
        private void OnUseSuccess()
        {
            // Print, Delete, Invoke Event
            Printer.PrintItemConsumed(Owner);
            World.Instance.RemoveEntity(Owner);
            OnConsumed?.Invoke(Owner);
        }
    }
}