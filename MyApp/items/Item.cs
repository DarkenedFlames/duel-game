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

        // Constructor requires an owner
        protected Item(Player owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public abstract void Use(Player target);

        public bool IsEquipment() =>
            Type == ItemType.Weapon ||
            Type == ItemType.Helmet ||
            Type == ItemType.Chestplate ||
            Type == ItemType.Leggings ||
            Type == ItemType.Accessory;
    }
}
