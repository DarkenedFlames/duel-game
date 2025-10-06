using System;
using System.Collections.Generic;
using System.Linq;

namespace MyApp
{
    public class Player
    {
        public event Action<Player, Item>? OnEquip;
        public event Action<Player, Item>? OnUnequip;
        public event Action<Player, Item>? OnReceiveItem;
        public event Action<Player, Item>? OnLoseItem;
        public event Action<Player, Item, Player>? OnUseItem;

        public Player(string newName)
        {
            Name = newName;

            Stats = new Stats();

            Resources = new Resources(Stats);

            Inventory = new List<Item>();
            ActiveEffects = new List<Effect>();
            Equipment = new Dictionary<ItemType, Item?>()
            {
                { ItemType.Weapon, null },
                { ItemType.Helmet, null },
                { ItemType.Chestplate, null },
                { ItemType.Leggings, null },
                { ItemType.Accessory, null }
            };

            // Event handlers
            this.OnEquip += (player, item) => Printer.PrintItemEquipped(player, item);
            this.OnUnequip += (player, item) => Printer.PrintItemUnequipped(player, item);
            this.OnReceiveItem += (player, item) => Printer.PrintItemReceived(player, item);
            this.OnLoseItem += (player, item) => Printer.PrintItemLost(player, item);
            this.OnUseItem += (player, item, target) => Printer.PrintItemUsed(player, item, target);
        }

        public string Name;
        public Stats Stats;
        public Resources Resources;
        public List<Item> Inventory;
        public Dictionary<ItemType, Item?> Equipment;
        public List<Effect> ActiveEffects;

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

        public void Heal(int amount) { /* Placeholder for future use */ }

        public void AddItem(Item item)
        {
            Inventory.Add(item);
            OnReceiveItem?.Invoke(this, item);
        }

        public void RemoveItem(Item item)
        {
            OnLoseItem?.Invoke(this, item);
            Console.WriteLine($"{Name} lost item: {item.Name}");
            Inventory.Remove(item);
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
                Inventory.Remove(item);

            return true;
        }

        public bool Equip(Item item)
        {
            if (!Inventory.Contains(item))
            {
                Console.WriteLine($"{Name} does not have item: {item.Name}");
                return false;
            }

            if (!Equipment.ContainsKey(item.Type))
            {
                Console.WriteLine($"{item.Name} cannot be equipped (type {item.Type} is not equippable).");
                return false;
            }

            if (Equipment[item.Type] != null)
            {
                var previous = Equipment[item.Type]!;
                Unequip(item.Type);
                Console.WriteLine($"{Name} unequipped {previous.Name}.");
            }

            // Equip the new item
            Equipment[item.Type] = item;
            Inventory.Remove(item);
            OnEquip?.Invoke(this, item);
            Console.WriteLine($"{Name} equipped {item.Name} ({item.Type}).");
            return true;
        }

        public bool Unequip(ItemType type)
        {
            if (!Equipment.ContainsKey(type))
            {
                Console.WriteLine($"No equipment slot for {type}.");
                return false;
            }

            var item = Equipment[type];
            if (item == null)
            {
                Console.WriteLine($"No {type} equipped.");
                return false;
            }

            Equipment[type] = null;
            AddItem(item);
            OnUnequip?.Invoke(this, item);
            Console.WriteLine($"{Name} unequipped {item.Name}.");
            return true;
        }

        public void ReceiveEffect(Effect newEffect)
        {
            var existing = ActiveEffects.FirstOrDefault(e => e.GetType() == newEffect.GetType());
            if (existing != null)
                existing.OnStack();
            else
            {
                ActiveEffects.Add(newEffect);
                newEffect.Receive();
            }
        }

        public void LoseEffect(Effect effect)
        {
            if (ActiveEffects.Contains(effect))
            {
                effect.Lose();
                ActiveEffects.Remove(effect);
            }
        }
    }
}
