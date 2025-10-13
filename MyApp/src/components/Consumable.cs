using System.Runtime.CompilerServices;

namespace CBA
{
    public class Consumable(Entity owner) : Component(owner)
    {
        public event Action<Entity>? OnConsumed;

        public override void Subscribe()
        {
            var usable = Helper.ThisIsNotNull(
                Owner.GetComponent<Usable>(),
                $"Consumable component requires Usable Component:" +
                $"\n\tItem entity: {Owner.GetComponent<ItemData>()?.Name}, " +
                $"\n\tPlayer entity: {World.Instance.GetPlayerOfItem(Owner)?.GetComponent<PlayerData>()?.Name}." 
            );
            usable.OnUseSuccess += (_, _) => OnUseSuccess();
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