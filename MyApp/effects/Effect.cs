using System;

namespace MyApp
{
    public abstract class Effect
    {
        public string Name { get; init; } = "";
        public Player Owner { get; }         // always exists
        public TargetType TargetType { get; init; }
        public int RemainingDuration { get; set; }
        public int MaximumDuration { get; set; }
        public int MaximumStacks { get; set; }
        public int RemainingStacks { get; set; }
        public bool IsNegative { get; set; }
        public bool IsExpired => RemainingDuration <= 0;

        protected Effect(Player owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public abstract void Tick();
        public abstract void Receive();
        public abstract void Lose();
    }
}
