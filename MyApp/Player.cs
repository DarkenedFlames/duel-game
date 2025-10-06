using System;
using System.Collections.Generic;
using System.Linq;

namespace MyApp
{
    public class Player
    {
        public event Action<Player, Item, Player>? OnUseItem;

        public Player(string newName)
        {
            Name = newName;

            Resources = new Resources(Stats);

            // Event handlers
            this.OnUseItem += (player, item, target) => Printer.PrintItemUsed(player, item, target);

            Inventory.OnEquipRequest += item => Equipment.Equip(item);

            Equipment.OnItemEquipped += item => Printer.PrintItemEquipped(this, item);
            Equipment.OnItemUnequipped += item => Printer.PrintItemUnequipped(this, item);
            Equipment.OnItemUnequipped += item => Inventory.AddItem(item);
            Equipment.OnItemEquipped += item => Inventory.RemoveItem(item);
            Inventory.OnItemAdded += item => Printer.PrintItemReceived(this, item);
            Inventory.OnItemRemoved += item => Printer.PrintItemLost(this, item);
        }

        public string Name;
        public Stats Stats = new();
        public Resources Resources;
        public Inventory Inventory = new();
        public Equipment Equipment = new();
        public ActiveEffects ActiveEffects = new();

        public bool TakeDamage(int amount, DamageType damageType, bool canCrit, bool canDodge, float critBonus = 0)
        {
            float dodgeChance = Stats.Get("Dodge") / (Stats.Get("Dodge") + 100f);
            if (new Random().NextDouble() < dodgeChance && canDodge)
            {
                Console.WriteLine($"{Name} dodged the attack!");
                return false;
            }

            float criticalChance = Stats.Get("Critical") / (Stats.Get("Critical") + 100f) + critBonus;
            if (new Random().NextDouble() < criticalChance && canCrit)
            {
                amount = (int)(amount * 2.0f);
                Console.WriteLine("Critical Hit!");
            }

            float multiplier;
            switch (damageType)
            {
                case DamageType.Physical:
                    multiplier = 100f / (Stats.Get("Armor") + 100f);
                    amount = (int)Math.Max(0, amount * multiplier);
                    break;
                case DamageType.Magical:
                    multiplier = 100f / (Stats.Get("Shield") + 100f);
                    amount = (int)Math.Max(0, amount * multiplier);
                    break;
                case DamageType.True:
                    break;
            }

            Resources.Change("Health", -amount);
            Console.WriteLine($"{Name} took {amount} damage!");
            return true;
        }
        public bool UseItem(Item item, Player target)
        {
            if (Resources.Get("Stamina") < item.StaminaCost)
            {
                Console.WriteLine($"{Name} does not have enough stamina to use {item.Name}.");
                return false;
            }

            Resources.Change("Stamina", -item.StaminaCost);
            item.Use(target);
            OnUseItem?.Invoke(this, item, target);

            if (item.Type == ItemType.Consumable)
                Inventory.RemoveItem(item);

            return true;
        }
    }
}
