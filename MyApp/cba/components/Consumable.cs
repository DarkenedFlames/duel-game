using System;

namespace CBA
{
    public class Consumable(Entity owner) : Component(owner)
    {
        public event Action<Entity>? OnConsumed;

        public override void Subscribe()
        {
            OnConsumed += Printer.PrintItemConsumed;

            var usable = Owner.GetComponent<Usable>();
            if (usable != null)
            {
                usable.OnUseSuccess += (user, target) =>
                {
                    World.Instance.RemoveEntity(Owner);  // remove item from world
                    OnConsumed?.Invoke(Owner);
                };
            }
        }
    }
}