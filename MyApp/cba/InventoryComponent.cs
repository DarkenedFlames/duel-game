using System;
using System.Collections.Generic;

namespace MyCBA
{
    public class InventoryComponent : Component
    {
        public event Action<Entity>? OnPickupItem;
        public event Action<Entity>? OnDropItem;

        private readonly List<Entity> _items = new();
        public IEnumerable<Entity> Items => _items;

        public void PickupItem(Entity item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
                OnPickupItem?.Invoke(item);
                Console.WriteLine($"{Owner?.Name} picked up {item.GetComponent<ItemComponent>()?.Name}.");
            }
        }

        public void DropItem(Entity item)
        {
            if (_items.Remove(item))
            {
                OnDropItem?.Invoke(item);
                Console.WriteLine($"{Owner?.Name} dropped {item.GetComponent<ItemComponent>()?.Name}.");
            }
        }

        public bool HasItem(Entity item) => _items.Contains(item);

        public void Subscribe(EquipmentComponent equipment)
        {
            equipment.OnEquip += (owner, item) =>
            {
                if (owner == Owner)
                    DropItem(item);
            };

            equipment.OnUnequip += (owner, item) =>
            {
                if (owner == Owner)
                    PickupItem(item);
            };
        }
    }
}
