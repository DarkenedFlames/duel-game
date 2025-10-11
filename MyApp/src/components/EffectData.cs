namespace CBA
{
    public enum StackingType { AddStack, RefreshOnly, Ignore }

    public class EffectData(Entity owner, Entity playerEntity,
                            string name,
                            bool isNegative = false,
                            bool isHidden = false,
                            StackingType stackingType = StackingType.AddStack,
                            int maxStacks = 1) : Component(owner)
    {
        public string Name { get; set; } = name;
        public Entity PlayerEntity { get; set; } = playerEntity;
        public bool IsNegative { get; set; } = isNegative;
        public bool IsHidden { get; set; } = isHidden;

        public int CurrentStacks { get; set; } = 1;
        public int MaximumStacks { get; set; } = maxStacks;
        public StackingType StackingType { get; set; } = stackingType;

        public override void Subscribe() { }
    }
}