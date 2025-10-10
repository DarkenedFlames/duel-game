using System;

namespace CBA
{
    public class EffectData(Entity owner, Entity playerEntity,
                            string name, bool isNegative = false,
                            bool isHidden = false) : Component(owner)
    {
        public string Name { get; set; } = name;
        public Entity PlayerEntity { get; set; } = playerEntity;
        public bool IsNegative { get; set; } = isNegative;
        public bool IsHidden { get; set; } = isHidden;

        protected override void Subscribe()
        {

        }
    }
}
