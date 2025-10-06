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


// Inventory.OnEquipItem += Equipment.HandleEquip;
// Inventory.OnUnequipItem += Equipment.HandleUnequip;

// Equipment.OnEquipped += item => Printer.PrintItemEquipped(this, item);
// Equipment.OnUnequipped += item => Printer.PrintItemUnequipped(this, item);

// Equipment.OnUnequipped += item => Inventory.AddItem(item);
// Equipment.OnEquipped += item => Inventory.RemoveItem(item);

// Inventory.OnAddItem += item => Printer.PrintItemReceived(this, item);
// Inventory.OnRemoveItem += item => Printer.PrintItemLost(this, item);