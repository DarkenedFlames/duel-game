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
            TargetType = TargetType.Self;
            IsNegative = false; // it’s a marker/buff
            IsHidden = true;    // hide from UI if you want
        }

        public override void Receive()
        {
            // Only a marker — no additional logic needed
        }

        public override void OnStack()
        {
            // If reapplied mid-turn, refresh duration
            RemainingDuration = MaximumDuration;
        }

        public override void Tick()
        {
            RemainingDuration--;
            if (RemainingDuration <= 0)
                Owner.LoseEffect(this);
        }

        public override void Lose()
        {
            // Optionally log
            // Console.WriteLine($"{Owner.Name}'s Moonlight has faded, next hit can crit again.");
        }
    }
}
