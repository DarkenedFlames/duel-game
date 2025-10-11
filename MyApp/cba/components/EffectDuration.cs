using System;

namespace CBA
{
    public class EffectDuration(Entity owner, int maxDuration) : Component(owner)
    {
        public int Remaining { get; set; } = maxDuration;
        public int Maximum { get; set; } = maxDuration;

        public override void Subscribe()
        {
            var target = Owner.GetComponent<EffectData>()?.PlayerEntity;
            var takesTurns = target?.GetComponent<TakesTurns>();
            if (takesTurns != null)
            {
                takesTurns.OnTurnStart += (player) => TickDuration();
            }
        }

        public void TickDuration()
        {
            if (Remaining <= 0)
            {
                World.Instance.RemoveEntity(Owner);
            }
            else
            {
                Remaining--;
            }
        }

    }
}
