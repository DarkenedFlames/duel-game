using System;

namespace CBA
{
    public class Consumable : Component
    {
        public event Action<Entity>? OnConsumed;

        public Consumable(Entity owner) : base(owner) { }

        protected override void Subscribe()
        {
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