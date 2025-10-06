using System;
using System.Collections.Generic;

namespace MyCBA
{
    public class EquipmentComponent : Component
    {
        public event Action<Entity?, Entity>? OnEquip;
        public event Action<Entity?, Entity>? OnUnequip;

        private readonly Dictionary<ItemType, Entity?> _equipment = new()
        {
            { ItemType.Weapon, null },
            { ItemType.Helmet, null },
            { ItemType.Chestplate, null },
            { ItemType.Leggings, null },
            { ItemType.Accessory, null }
        };

        public IReadOnlyDictionary<ItemType, Entity?> Equipped => _equipment;

        public bool Equip(Entity item)
        {
            var itemComp = item.GetComponent<ItemComponent>();
            if (itemComp == null)
            {
                Console.WriteLine("Tried to equip an entity that isn't an item.");
                return false;
            }

            if (!_equipment.ContainsKey(itemComp.Type))
            {
                Console.WriteLine($"{itemComp.Name} cannot be equipped (type {itemComp.Type} not supported).");
                return false;
            }

            if (_equipment[itemComp.Type] != null)
            {
                Unequip(itemComp.Type);
            }

            _equipment[itemComp.Type] = item;
            OnEquip?.Invoke(Owner, item);
            Console.WriteLine($"{Owner?.Name} equipped {itemComp.Name} ({itemComp.Type}).");
            return true;
        }

        public bool Unequip(ItemType slot)
        {
            if (!_equipment.TryGetValue(slot, out var item) || item == null)
            {
                Console.WriteLine($"No {slot} equipped.");
                return false;
            }

            _equipment[slot] = null;
            OnUnequip?.Invoke(Owner, item);
            Console.WriteLine($"{Owner?.Name} unequipped {item.GetComponent<ItemComponent>()?.Name}.");
            return true;
        }

        public bool IsEquipped(ItemType slot) =>
            _equipment.TryGetValue(slot, out var item) && item != null;

        public Entity? GetEquipped(ItemType slot)
        {
            _equipment.TryGetValue(slot, out var item);
            return item;
        }
    }
}
