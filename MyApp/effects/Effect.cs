using System;

namespace MyApp
{
    public abstract class Effect(Player owner)
    {
        public string Name { get; init; } = "";
        public Player Owner { get; } = owner;
        public StackingType StackingType { get; init; } = StackingType.AddStack;
        public int RemainingDuration { get; set; }
        public int MaximumDuration { get; set; }
        public int MaximumStacks { get; set; }
        public int RemainingStacks { get; set; }
        public bool IsNegative { get; set; }
        public bool IsHidden { get; set; }

        // Effects subscribe to container events dynamically
        public virtual void Subscribe(ActiveEffects container)
        {
            container.OnEffectAdded += e => { if (e == this) OnAdded(); };
            container.OnEffectStacked += e => { if (e == this) OnStacked(); };
            container.OnEffectRemoved += e => { if (e == this) OnRemoved(); };
            container.OnEffectTicked += e => { if (e == this) OnTick(); };
        }

        // These are the “reactions” an effect can define
        protected virtual void OnAdded() { }
        protected virtual void OnStacked() { }
        protected virtual void OnRemoved() { }
        protected virtual void OnTick() { }
    }
}
