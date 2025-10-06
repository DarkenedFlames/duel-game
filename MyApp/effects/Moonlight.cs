using System;

namespace MyApp
{
    public class Moonlight : Effect
    {
        public Moonlight(Player owner) : base(owner)
        {
            Name = "Moonlight";
            MaximumDuration = 1;
            RemainingDuration = 1;
            MaximumStacks = 1;
            RemainingStacks = 1;
            StackingType = StackingType.RefreshOnly;
            IsHidden = true;
        }

        protected override void OnAdded()
        {
            Console.WriteLine($"{Owner.Name} is bathed in moonlight.");
        }

        protected override void OnRemoved()
        {
            Console.WriteLine($"{Owner.Name}'s Moonlight fades.");
        }
    }
}
