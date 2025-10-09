using System;

namespace CBA
{
    public class EffectData : Component
    {
        public string Name { get; set; } = "";
        public Entity PlayerEntity { get; set; }
        public bool IsNegative { get; set; } = false;
        public bool IsHidden { get; set; } = false;

        public EffectData(Entity owner, Entity playerEntity, string name, Entity? owningEntity, bool isNegative = false, bool isHidden = false) 
            : base(owner)
        {
            Name = name;
            PlayerEntity = playerEntity;
            IsNegative = isNegative;
            IsHidden = isHidden;
        }

        protected override void Subscribe()
        {

        }
    }
}
