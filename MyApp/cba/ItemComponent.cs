using System;

namespace MyCBA
{
    public class ItemComponent : Component
    {
        public string Name { get; }
        public ItemType Type { get; }
        public ItemRarity Rarity { get; }

        public ItemComponent(string name, ItemType type, ItemRarity rarity)
        {
            Name = name;
            Type = type;
            Rarity = rarity;
        }
    }

    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Mythical
    }

    public enum ItemType
    {
        Weapon,
        Helmet,
        Chestplate,
        Leggings,
        Accessory,
        Consumable
    }
}
