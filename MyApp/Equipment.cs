using System;
using System.Collections.Generic;

namespace MyApp
{
    public class Equipment
    {
        public event Action<Item>? OnItemEquipped;
        public event Action<Item>? OnItemUnequipped;

        private readonly Dictionary<ItemType, Item?> _slots = new()
        {
            { ItemType.Weapon, null },
            { ItemType.Helmet, null },
            { ItemType.Chestplate, null },
            { ItemType.Leggings, null },
            { ItemType.Accessory, null }
        };

        public IReadOnlyDictionary<ItemType, Item?> Slots => _slots;

        // Fires OnItemEquipped event if item was equipped
        // Fired by Inventory.OnEquipRequest
        public void Equip(Item item)
        {
            if (!_slots.ContainsKey(item.Type))
            {
                Console.WriteLine($"{item.Name} cannot be equipped (invalid type).");
                return;
            }

            // Unequip current item in the slot if present
            if (_slots[item.Type] is Item oldItem)
            {
                _slots[item.Type] = null;
                Unequip(item.Type);
            }

            _slots[item.Type] = item;
            OnItemEquipped?.Invoke(item);
        }

        // Fires OnItemUnequipped event if an item was unequipped
        public void Unequip(ItemType type)
        {
            if (!_slots.ContainsKey(type) || _slots[type] is not Item item)
            {
                Console.WriteLine($"Nothing equipped in {type} slot.");
                return;
            }

            _slots[type] = null;
            OnItemUnequipped?.Invoke(item);
        }
    }
}