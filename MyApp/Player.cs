using System;
using System.Collections.Generic;

namespace MyApp
{
    public class Player
    {
        // Place Holder: public event Action<Player, int>? OnTakeDamage;
        // Place Holder: public event Action<Player, int>? OnHeal;
        public event Action<Player, Item>? OnEquip;
        public event Action<Player, Item>? OnUnequip;
        public event Action<Player, Item>? OnReceiveItem;
        public event Action<Player, Item>? OnLoseItem;
        public event Action<Player, Item, Player>? OnUseItem;

        public Player(string newName)
        {
            Name = newName;

            MaximumHealth = new MaximumHealth();
            MaximumStamina = new MaximumStamina();
            Health = new Health(MaximumHealth);
            Stamina = new Stamina(MaximumStamina);
            Armor = new Armor();
            Shield = new Shield();
            Critical = new Critical();
            Dodge = new Dodge();
            Peer = new Peer();
            Luck = new Luck();


            Inventory = new List<Item>();
            ActiveEffects = new List<Effect>();
            Equipment = new Dictionary<EquipmentSlot, Item?>()
            {
                { EquipmentSlot.Weapon, null },
                { EquipmentSlot.Helmet, null },
                { EquipmentSlot.Chestplate, null },
                { EquipmentSlot.Leggings, null },
                { EquipmentSlot.Accessory, null }
            };

            // Register/Subscribe Printers For The Instance
            this.OnEquip += (player, item) => Printer.PrintItemEquipped(player, item);
            this.OnUnequip += (player, item) => Printer.PrintItemUnequipped(player, item);
            this.OnReceiveItem += (player, item) => Printer.PrintItemReceived(player, item);
            this.OnLoseItem += (player, item) => Printer.PrintItemLost(player, item);
            this.OnUseItem += (player, item, target) => Printer.PrintItemUsed(player, item, target);
        }

        public string Name;
        public MaximumHealth MaximumHealth;
        public MaximumStamina MaximumStamina;
        public Health Health;
        public Stamina Stamina;
        public Armor Armor;
        public Shield Shield;
        public Critical Critical;
        public Dodge Dodge;
        public Peer Peer;
        public Luck Luck;

        public List<Item> Inventory;
        public Dictionary<EquipmentSlot, Item?> Equipment;
        public List<Effect> ActiveEffects;

        // Player take damage, apply armor/shield, crits, dodges
        public bool TakeDamage(int amount, DamageType damageType, bool canCrit, bool canDodge, float critBonus = 0)
        {
            float dodgeChance = Dodge.Value / (Dodge.Value + 100f);
            if (new Random().NextDouble() < dodgeChance && canDodge)
            {
                Console.WriteLine($"{Name} dodged the attack!");
                return false;
            }

            float criticalChance = Critical.Value / (Critical.Value + 100f) + critBonus;
            if (new Random().NextDouble() < criticalChance && canCrit)
            {
                amount = (int)(amount * 2.0f); // Critical hit deals 100% more damage
                Console.WriteLine("Critical Hit!");
            }

            float multiplier;
            switch (damageType)
            {
                case DamageType.Physical:
                    multiplier = 100f / (Armor.Value + 100f);
                    amount = (int)Math.Max(0, amount * multiplier);
                    break;
                case DamageType.Magical:
                    multiplier = 100f / (Shield.Value + 100f);
                    amount = (int)Math.Max(0, amount * multiplier);
                    break;
                case DamageType.True:
                    // True damage ignores armor
                    break;
            }

            Health.Change(-amount);
            Console.WriteLine($"{Name} took {amount} damage!");
            return true;
        }
        // Player heal, not implemented
        public void Heal(int amount)
        {
            //Health.Change(amount);
            //Console.WriteLine($"{Name} healed {amount} health!");
            //Might be used later, needs event triggers to justify existence
        }
        // Add Item to inventory, call Item.Receive()
        public void AddItem(Item item)
        {
            Inventory.Add(item);
            OnReceiveItem?.Invoke(this, item);
        }
        // Remove Item from inventory, call Item.Lose()
        public void RemoveItem(Item item)
        {
            OnLoseItem?.Invoke(this, item);
            Console.WriteLine($"{Name} lost item: {item.Name}");
            Inventory.Remove(item);
        }
        // Use item, check and spend stamina, call Item.Use()
        public bool UseItem(Item item, Player target)
        {

            // Check stamina
            if (Stamina.Value < item.StaminaCost)
            {
                Console.WriteLine($"{Name} does not have enough stamina to use {item.Name}.");
                return false;
            }
            else
            {
                Stamina.Change(-item.StaminaCost);
                item.Use(target);
                OnUseItem?.Invoke(this, item, target);
                if (item.Type == ItemType.Consumable){Inventory.Remove(item);}
                return true;
            }
        }

        // Equip an item or perform logic based on ItemType, TODO: add effect application and stat changes
        public bool Equip(Item item)
        {
            if (!Inventory.Contains(item))
            {
                Console.WriteLine($"{Name} does not have item: {item.Name}");
                return false;
            }

            EquipmentSlot slot;
            switch (item.Type)
            {
                case ItemType.Weapon:
                    slot = EquipmentSlot.Weapon;
                    break;
                case ItemType.Helmet:
                    slot = EquipmentSlot.Helmet;
                    break;
                case ItemType.Chestplate:
                    slot = EquipmentSlot.Chestplate;
                    break;
                case ItemType.Leggings:
                    slot = EquipmentSlot.Leggings;
                    break;
                case ItemType.Accessory:
                    slot = EquipmentSlot.Accessory;
                    break;
                case ItemType.Consumable:
                    Console.WriteLine($"{item.Name} is a consumable and cannot be equipped.");
                    return false;
                default:
                    Console.WriteLine($"{item.Name} cannot be equipped (unknown type).");
                    return false;
            }

            Inventory.Remove(item);
            if (Equipment[slot] != null)
            {
                Inventory.Add(Equipment[slot]!);
                Console.WriteLine($"{Name} unequipped {Equipment[slot]?.Name} from {slot} slot.");
            }
            Equipment[slot] = item;
            OnEquip?.Invoke(this, item);
            return true;
        }
        // Unequip logic based on ItemType; TODO: Add effect application and stat changes
        public bool Unequip(EquipmentSlot slot)
        {
            var item = Equipment[slot];
            if (item == null)
            {
                Console.WriteLine($"No item equipped in {slot} slot.");
                return false;
            }

            switch (item.Type)
            {
                case ItemType.Weapon:
                case ItemType.Helmet:
                case ItemType.Chestplate:
                case ItemType.Leggings:
                case ItemType.Accessory:
                    AddItem(item);
                    Equipment[slot] = null;
                    OnUnequip?.Invoke(this, item);
                    return true;
                default:
                    Console.WriteLine($"{item.Name} cannot be unequipped (unknown type).");
                    return false;
            }
        }
        // Apply effect to player
        public void ReceiveEffect(Effect newEffect)
        {
            // Try to merge with existing effect of same type
            var existing = ActiveEffects.FirstOrDefault(e => e.GetType() == newEffect.GetType());

            if (existing != null)
            {
                // Ask the effect how to handle stacking/refreshing
                existing.OnStack();
            }
            else
            {
                ActiveEffects.Add(newEffect);
                newEffect.Receive();
            }
        }

        // Remove effect from player
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