using System;

namespace MyApp
{
    public class Inferno : Effect
    {
        public Inferno(Player owner) : base(owner)
        {
            Name = "Inferno";
            MaximumDuration = 3;
            RemainingDuration = 3;
            MaximumStacks = 3;
            RemainingStacks = 1;
            StackingType = StackingType.AddStack;
            IsNegative = true;
        }

        protected override void OnAdded()
        {
            Console.WriteLine($"{Owner.Name} is engulfed in flames!");
        }

        protected override void OnStacked()
        {
            Console.WriteLine($"{Owner.Name}'s Inferno stacks to {RemainingStacks}!");
        }

        protected override void OnTick()
        {
            int damage = Math.Max(1, (int)(Owner.Stats.Get("MaximumHealth") * 0.01f)) * RemainingStacks;
            Owner.TakeDamage(damage, DamageType.Magical, canCrit: false, canDodge: false);
            Console.WriteLine($"{Owner.Name} burns ({RemainingDuration - 1} turns left).");
        }

        protected override void OnRemoved()
        {
            Console.WriteLine($"{Owner.Name} is no longer aflame.");
        }
    }
}
