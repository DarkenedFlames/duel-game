using System;

namespace MyApp
{
    public class VultureVittles : Item
    {
        public int PeerAmount { get; init; } = 5;

        public VultureVittles(Player owner) : base(owner)
        {
            Name = "Vulture Vittles";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Common;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Stats["Peer"].Base += PeerAmount;
            Console.WriteLine($"{target.Name} gained {PeerAmount} peer!");
        }
    }
}
