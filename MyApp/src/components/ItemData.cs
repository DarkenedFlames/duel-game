namespace CBA
{
    public class ItemData(Entity owner, string name, ItemRarity rarity, ItemType type, Entity playerEntity) : Component(owner)
    {
        public string Name { get; init; } = name;
        public ItemRarity Rarity { get; init; } = rarity;
        public ItemType Type { get; init; } = type;
        public Entity PlayerEntity { get; init; } = playerEntity;

        public override void Subscribe() { }
    }

}
