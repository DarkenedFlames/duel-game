using System;

namespace CBA
{
    public class ItemData : Component
    {
        public string Name { get; init; }
        public ItemRarity Rarity { get; init; }
        public ItemType Type { get; init; }
        public Entity PlayerEntity { get; init; }  // owning player

        public ItemData(Entity owner, string name, ItemRarity rarity, ItemType type, Entity playerEntity) 
            : base(owner)
        {
            Name = name;
            Rarity = rarity;
            Type = type;
            PlayerEntity = playerEntity;
        }

        protected override void Subscribe()
        {
            // World entity added event printing
        }
    }

}
