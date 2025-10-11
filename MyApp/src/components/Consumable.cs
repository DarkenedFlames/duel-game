using System;

namespace CBA
{
    public class Consumable(Entity owner) : Component(owner)
    {
        public event Action<Entity>? OnConsumed;

        public override void Subscribe()
        {
            var usable = Owner.GetComponent<Usable>();
            if (usable != null)
            {
                usable.OnUseSuccess += (user, target) =>
                {
                    Printer.PrintItemConsumed(Owner);
                    World.Instance.RemoveEntity(Owner);  // remove item from world
                    OnConsumed?.Invoke(Owner);
                };
            }
        }
    }
}