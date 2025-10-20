namespace CBA
{
    public class ItemData(Entity owner,
                        Entity playerEntity,
                        ItemRarity rarity,
                        ItemType type
                        ) : Component(owner)
    {
        public ItemRarity Rarity { get; init; } = rarity;
        public ItemType Type { get; init; } = type;
        public Entity PlayerEntity { get; init; } = playerEntity;

        public override void Subscribe() { }
    }

}
