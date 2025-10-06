using System;
using System.Collections.Generic;
using MyApp;

namespace MyApp
{
    public class Inventory
    {
        public event Action<Item>? OnItemAdded;
        public event Action<Item>? OnItemRemoved;
        public event Action<Item>? OnEquipRequest;

        private readonly List<Item> _items = new();

        public IReadOnlyList<Item> Items => _items.AsReadOnly();

        public void AddItem(Item item)
        {
            _items.Add(item);
            OnItemAdded?.Invoke(item);
        }

        public bool RemoveItem(Item item)
        {
            if (_items.Remove(item))
            {
                OnItemRemoved?.Invoke(item);
                return true;
            }
            return false;
        }

        public bool Contains(Item item) => _items.Contains(item);

        public void TryEquip(Item item)
        {
            if (!Contains(item))
            {
                Console.WriteLine($"Cannot equip {item.Name}: not in inventory.");
                return;
            }
            OnEquipRequest?.Invoke(item);
        }
    }
}
// Inventory.OnEquipRequest += Equipment.Equip;

// Equipment.OnItemEquipped += item => Printer.PrintItemEquipped(this, item);
// Equipment.OnItemUnequipped += item => Printer.PrintItemUnequipped(this, item);

// Equipment.OnItemUnequipped += item => Inventory.AddItem(item);
// Equipment.OnItemEquipped += item => Inventory.RemoveItem(item);


// inventory.tryequip -> onequiprequest -> equipment.equip -> equipment.onitemequipped -> inventory.removeitem