using System;

namespace CBA
{
    public class EffectDuration : Component
    {
        public int Remaining { get; set; }
        public int Maximum { get; set; }

        public EffectDuration(Entity owner, int maxDuration) : base(owner)
        {
            Maximum = maxDuration;
            Remaining = maxDuration;
        }

        protected override void Subscribe() { }
    }
}
