using System;
using System.Collections.Generic;

namespace MyApp
{
    public abstract class Item
    {
        public string Name { get; init; } = "";
        public Player Owner { get; }
        public ItemRarity Rarity { get; init; }
        public ItemType Type { get; init; }
        public DamageType DamageType { get; init; } = DamageType.Physical;
        public bool CanCrit { get; init; } = false;
        public bool CanDodge { get; init; } = false;
        public int Damage { get; init; } = 0;
        public int StaminaCost { get; init; } = 0;

        public event Action<Item, Player>? OnUsed;
        public event Action<Item, Player>? OnInsufficientStamina;

        // Constructor requires an owner
        protected Item(Player owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Subscribe();
        }

        private void Subscribe()
        {
            // Example of auto-registration:
            // When owner uses an item, print it (universal logic)
            OnUsed += (item, target) => Printer.PrintItemUsed(Owner, item, target);
            OnInsufficientStamina += (item, target) => Printer.PrintInsufficientStamina(Owner, item);
        }

        public void TryUse(Player target)
        {
            if (Owner.Resources.Get("Stamina") < StaminaCost)
            {
                OnInsufficientStamina?.Invoke(this, target);
                return;
            }

            Owner.Resources.Change("Stamina", -StaminaCost);
            Use(target);
            OnUsed?.Invoke(this, target);
            if (Type == ItemType.Consumable)
                Owner.Inventory.RemoveItem(this);
        }

        protected abstract void Use(Player target);

        public bool IsEquipment() =>
            Type == ItemType.Weapon ||
            Type == ItemType.Helmet ||
            Type == ItemType.Chestplate ||
            Type == ItemType.Leggings ||
            Type == ItemType.Accessory;
    }
}
