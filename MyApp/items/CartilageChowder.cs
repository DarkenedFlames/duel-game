using System;

namespace MyApp
{
    public class CartilageChowder : Item
    {
        public int DodgeAmount { get; init; } = 5;

        public CartilageChowder(Player owner) : base(owner)
        {
            Name = "Cartilage Chowder";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Common;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Stats["Dodge"].Base += DodgeAmount;
            Console.WriteLine($"{target.Name} gained {DodgeAmount} dodge!");
        }
    }
}
