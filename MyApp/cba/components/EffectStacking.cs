using System;
using System.Linq;

namespace CBA
{
    public enum StackingType { AddStack, RefreshOnly, Ignore }

    public class EffectStacking : Component
    {
        public StackingType Type { get; set; } = StackingType.AddStack;
        public int MaximumStacks { get; set; } = 1;
        public int CurrentStacks { get; set; } = 1;

        public EffectStacking(Entity owner, StackingType type = StackingType.AddStack, int maxStacks = 1) : base(owner)
        {
            Type = type;
            MaximumStacks = maxStacks;
        }

        protected override void Subscribe() { }

        // Call this when applying the effect to a player
        public void HandleStacking(Entity targetOwner)
        {
            // Find existing effect of the same type on this player
            var existing = World.Instance.GetEntitiesWith<EffectData>()
                .Select(e => e.GetComponent<EffectData>()!)
                .FirstOrDefault(ed => ed.PlayerEntity == targetOwner &&
                                      ed.Name == Owner.GetComponent<EffectData>()!.Name);

            if (existing != null)
            {
                var stackingComp = existing.Owner.GetComponent<EffectStacking>()!;
                switch (stackingComp.Type)
                {
                    case StackingType.RefreshOnly:
                        existing.Owner.GetComponent<EffectDuration>()!.Remaining =
                            existing.Owner.GetComponent<EffectDuration>()!.Maximum;
                        break;
                    case StackingType.AddStack:
                        if (stackingComp.CurrentStacks < stackingComp.MaximumStacks)
                            stackingComp.CurrentStacks++;
                        existing.Owner.GetComponent<EffectDuration>()!.Remaining =
                            existing.Owner.GetComponent<EffectDuration>()!.Maximum;
                        break;
                    case StackingType.Ignore:
                        World.Instance.RemoveEntity(Owner);
                        break;
                }
            }
        }
    }
}
