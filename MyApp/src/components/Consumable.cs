namespace CBA
{
    public class Consumable(Entity owner) : Component(owner)
    {
        public event Action<Entity>? OnConsumed;

        public override void Subscribe()
        {
            Owner.GetComponent<Usable>()?.OnUseSuccess += (user, target) =>
            {
                Printer.PrintItemConsumed(Owner);
                World.Instance.RemoveEntity(Owner);
                OnConsumed?.Invoke(Owner);
            };
        }
    }
}