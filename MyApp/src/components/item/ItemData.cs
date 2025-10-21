namespace CBA
{
    public enum ItemRarity { Common, Uncommon, Rare, Mythical }
    public enum ItemType { Consumable, Weapon, Armor, Accessory }
    public class ItemData(
                        Entity owner,
                        Entity playerEntity,
                        ItemRarity rarity,
                        ItemType type
                        ) : Component(owner)
    {
        public ItemRarity Rarity { get; init; } = rarity;
        public ItemType Type { get; init; } = type;
        public Entity PlayerEntity { get; init; } = playerEntity;
        protected override void RegisterSubscriptions(){}
    }

}
