// ==================== Printer ====================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace CBA
{
    public static class Printer
    {
        // General helpers
        public static void ClearAndHeader(string header)
        {
            Console.Clear();
            Console.WriteLine(new string('-', header.Length));
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));
        }

        public static void PrintMessage(string message) => Console.WriteLine(message);

        // --- Menu helpers ---
        public static void PrintMenu(List<string> options)
        {
            for (int i = 0; i < options.Count; i++)
                Console.WriteLine($"{i + 1}. {options[i]}");
        }

        // --- Stats ---
        public static void PrintStats(Entity player)
        {
            ResourcesComponent resources = player.GetComponent<ResourcesComponent>();
            StatsComponent stats = player.GetComponent<StatsComponent>();

            Console.WriteLine($"Health: {resources.Get("Health")} / {stats.Get("MaximumHealth")}");
            Console.WriteLine($"Stamina: {resources.Get("Stamina")} / {stats.Get("MaximumStamina")}");
            Console.WriteLine($"Armor: {stats.Get("Armor")}");
            Console.WriteLine($"Shield: {stats.Get("Shield")}");
            Console.WriteLine($"Critical: {stats.Get("Critical")}");
            Console.WriteLine($"Dodge: {stats.Get("Dodge")}");
            Console.WriteLine($"Peer: {stats.Get("Peer")}");
            Console.WriteLine($"Luck: {stats.Get("Luck")}");
            Console.WriteLine($"Healing Modifier: {resources.GetRestoreMultiplier("Health"):P}");
            Console.WriteLine($"Stimming Modifier: {resources.GetRestoreMultiplier("Stamina"):P}");
        }

        // --- Status Effects ---
        public static void PrintEffects(Entity player)
        {
            IEnumerable<Entity> effects = World.Instance.GetAllForPlayer<Entity>(player, EntityCategory.Effect);

            if (!effects.Any()) { Console.WriteLine("(No active effects)"); return; }

            foreach (Entity effect in effects)
            {
                if (effect.HasComponent<EffectDuration>())
                    Console.WriteLine($"- {effect.DisplayName} (Duration: {effect.GetComponent<EffectDuration>().Remaining} turns)");
                else
                    Console.WriteLine($"- {effect.DisplayName}");
            }
        }

        // --- Items ---
        public static void PrintItemList(IEnumerable<Entity> items)
        {
            int idx = 1;
            foreach (Entity? item in items)
            {
                string? name = item.GetComponent<ItemData>()?.Name;
                Console.WriteLine($"{idx}. {name}");
                idx++;
            }
            if (idx == 1) Console.WriteLine("(No items found)");
        }

        // --- Equipment ---
        public static List<Entity> PrintEquipment(Entity player)
        {
            List<Entity>? equippedItems = [.. World.Instance.GetEntitiesWith<Wearable>()
                .Where(e =>
                {
                    Wearable? wearable = e.GetComponent<Wearable>();
                    ItemData? itemData = e.GetComponent<ItemData>();
                    return wearable != null &&
                           itemData != null &&
                           wearable.IsEquipped &&
                           itemData.PlayerEntity == player;
                })];

            List<(EquipType type, string label)>? slots =
            [
                (EquipType.Weapon, "Weapon"),
                (EquipType.Helmet, "Helmet"),
                (EquipType.Chestplate, "Chestplate"),
                (EquipType.Leggings, "Leggings"),
                (EquipType.Accessory, "Accessory")
            ];

            List<Entity>? orderedEquipment = [];
            Console.WriteLine("Equipment:");
            for (int i = 0; i < slots.Count; i++)
            {
                (EquipType type, string label) = slots[i];
                Entity? item = equippedItems.FirstOrDefault(e => e.GetComponent<Wearable>()?.EquipType == type);
                string? itemName = item?.GetComponent<ItemData>()?.Name;
                Console.WriteLine($"{i + 1}. {label}: {itemName}");
                if (item != null) orderedEquipment.Add(item);
            }

            return orderedEquipment;
        }

        // --- Item Menu ---
        public static int? PrintItemMenu(Entity item)
        {
            ItemData itemData = item.GetComponent<ItemData>();
            Entity player = itemData.PlayerEntity;
            
            ClearAndHeader($"Item Menu: {player.DisplayName} using {item.DisplayName}");

            List<string> actions = ["Remove"];

            if (item.HasComponent<Usable>() & !item.HasComponent<Wearable>())
                actions.Add("Use");

            if (item.HasComponent<Wearable>())
            {
                if (item.GetComponent<Wearable>().IsEquipped)
                {
                    actions.Add("Unequip");
                    if (item.HasComponent<Usable>()) actions.Add("Use");
                }
                else actions.Add("Equip");
            }

            for (int i = 0; i < actions.Count; i++)
                Console.WriteLine($"{i + 1}. {actions[i]}");

            Console.Write("Select an action: ");
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int idx) && idx >= 1 && idx <= actions.Count)
                return idx;

            Console.WriteLine("Invalid choice.");
            return null;
        }

        // --- Targets ---
        public static void PrintTargetList(List<Entity> targets)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                var name = targets[i].DisplayName;
                Console.WriteLine($"{i + 1}. {name}");
            }
        }

        public static string? MultiChoiceList(List<string> labels)
        {
            for (int i = 0; i < labels.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {labels[i]}");
            }
            int idx = InputHandler.GetNumberInput(labels.Count, "Select a number: ") - 1;
            if (idx == 0)
                return null;
            return labels[0];
        }

        //================== Event Printers =================//
        public static void PrintTurnStartHeader(Entity player)
        {
            Console.Clear();
            Console.WriteLine($"-----{player.DisplayName}'s turn has began!-----");
        }
        public static void PrintTurnEndHeader(Entity player)
        {
            Console.WriteLine($"-----{player.DisplayName}'s turn has ended!-----");
        }


        public static void PrintEntityAdded(Entity entity)
        {
            switch (entity.Id.Category)
            {
                case EntityCategory.Player:
                    Console.WriteLine($"\n{entity.DisplayName} has joined the game!");
                    break;
                case EntityCategory.Item:
                    Console.WriteLine($"\n{entity.GetComponent<ItemData>().PlayerEntity.DisplayName} has picked up {entity.DisplayName}!");
                    break;
                case EntityCategory.Effect:
                    string playerName = entity.GetComponent<EffectData>().PlayerEntity.DisplayName;
                    string effectName = entity.DisplayName;
                    if (entity.HasComponent<EffectDuration>())
                    {
                        EffectDuration effectDuration = entity.GetComponent<EffectDuration>();
                        Console.WriteLine($"\n{playerName} has gained an effect: {effectName}! Remaining turns: {effectDuration.Remaining}");
                    }
                    else
                    {
                        Console.WriteLine($"\n{playerName} has gained an effect: {effectName}!");
                    }
                    break;
                default:
                    throw new Exception($"Printer couldn't find type of entity removed.");
            }
        }
        public static void PrintEntityRemoved(Entity entity)
        {
            switch (entity.Id.Category)
            {
                case EntityCategory.Player:
                    Console.WriteLine($"\n{entity.DisplayName} has died!");
                    break;
                case EntityCategory.Item:
                    Console.WriteLine($"\n{entity.GetComponent<ItemData>().PlayerEntity.DisplayName} has lost {entity.DisplayName}!");
                    break;
                case EntityCategory.Effect:
                    string playerName = entity.GetComponent<EffectData>().PlayerEntity.DisplayName;
                    string effectName = entity.DisplayName;

                    if (entity.HasComponent<EffectDuration>())
                    {
                        Console.WriteLine($"\n{playerName}'s effect: {effectName} has expired!");
                    }
                    else
                    {
                        Console.WriteLine($"\n{playerName} has lost an effect: {effectName}!");
                    }
                    break;
                default:
                    throw new Exception($"Printer couldn't find type of entity removed.");
            }
        }

        public static void PrintItemEquipped(Entity item)
        {
            Console.WriteLine($"\n{item.GetComponent<ItemData>().PlayerEntity.DisplayName} equipped {item.DisplayName}!");
        }
        public static void PrintItemUnequipped(Entity item)
        {
            Console.WriteLine($"\n{item.GetComponent<ItemData>().PlayerEntity.DisplayName} unequipped {item.DisplayName}!");
        }
        public static void PrintItemUsed(Entity item, Entity target)
        {
            
            Entity user = item.GetComponent<ItemData>().PlayerEntity;
            
            string itemName = item.DisplayName;
            string userName = user.DisplayName;
            string targetName = target.DisplayName;

            if (user == target)
            {
                Console.WriteLine($"\n{userName} used {itemName} on themselves!");
            }
            else
            {
                Console.WriteLine($"\n{userName} used {itemName} on {targetName}!");
            }

        }
        public static void PrintItemConsumed(Entity item)
        {
            string itemName = item.DisplayName;
            string playerName = item.GetComponent<ItemData>().PlayerEntity.DisplayName;
            Console.WriteLine($"\n{playerName} consumed their {itemName}!");
        }

        public static void PrintInsufficientStamina(Entity item)
        {
            Console.WriteLine($"\n{item.GetComponent<ItemData>().PlayerEntity.DisplayName} lacks stamina to use {item.DisplayName}.");
        }

        public static void PrintStatChanged(StatsComponent stats, string statName)
        {
            Console.WriteLine($"\n{stats.Owner.DisplayName}'s {statName} changed to {stats.Get(statName)}.");
        }
        public static void PrintResourceChanged(ResourcesComponent resources, string resourceName)
        {
            Console.WriteLine($"\n{resources.Owner.DisplayName}'s {resourceName} is now {resources.Get(resourceName)}.");
        }
        public static void PrintResourceDepleted(ResourcesComponent resources, string resourceName)
        {
            Console.WriteLine($"\n{resources.Owner.DisplayName}'s {resourceName} has been depleted!");
        }

        public static void PrintDamageDealt(Entity itemOrEffect, Entity target, int finalDamage)
        {
            string userName;
            string sourceName = itemOrEffect.DisplayName;
            string targetName = target.DisplayName;

            if (itemOrEffect.Id.Category == EntityCategory.Item)
            {
                if (itemOrEffect.GetComponent<ItemData>().PlayerEntity == target)
                {
                    userName = targetName;
                    Console.WriteLine($"\n{userName} damaged themselves with {sourceName} for {finalDamage} damage!");
                }
                else
                {
                    userName = itemOrEffect.GetComponent<ItemData>().PlayerEntity.DisplayName;
                    Console.WriteLine($"\n{userName} dealt {finalDamage} damage to {targetName} with {sourceName}!");
                }
            }
            else if (itemOrEffect.Id.Category == EntityCategory.Effect)
            {
                sourceName = itemOrEffect.DisplayName;
                Console.WriteLine($"\n{targetName} took {finalDamage} damage from {sourceName}!");
            }
        }

        public static void PrintDodged(Entity itemOrEffect, Entity target)
        {
            string userName;
            string targetName = target.DisplayName;
            string sourceName = itemOrEffect.DisplayName;

            if (itemOrEffect.Id.Category == EntityCategory.Item)
            {
                userName = itemOrEffect.GetComponent<ItemData>().PlayerEntity.DisplayName;
                Console.WriteLine($"\n{userName}'s attack with {sourceName} was dodged by {targetName}!");

            }
            else if (itemOrEffect.Id.Category == EntityCategory.Effect)
            {
                Console.WriteLine($"\n{targetName} dodged damage from their {sourceName} effect!");
            }
            else
            {
                throw new NullReferenceException($"{itemOrEffect} was null when it should have been an <Entity>.");
            }
        }

        public static void PrintCritical(Entity itemOrEffect, Entity target)
        {
            string userName;
            string targetName = target.DisplayName;
            string sourceName = itemOrEffect.DisplayName;

            if (itemOrEffect.Id.Category == EntityCategory.Item)
            {
                userName = itemOrEffect.GetComponent<ItemData>().PlayerEntity.DisplayName;
                Console.WriteLine($"\n{userName}'s attack with {sourceName} scored a critical hit on {targetName}!");

            }
            else if (itemOrEffect.Id.Category == EntityCategory.Effect)
            {
                Console.WriteLine($"\n{targetName} was critically damaged by their {sourceName} effect!");
            }
            else
            {
                throw new NullReferenceException($"{itemOrEffect} was null when it should have been an <Entity>.");
            }
        }

        public static void PrintPeered(List<string> targetItems, Entity target)
        {
            Console.WriteLine($"\nPeer activated! {target.DisplayName}'s inventory:");
            Console.WriteLine(string.Join(", ", targetItems));
            InputHandler.WaitForKey();
        }
        /*
        public static void PrintEffectStacked(Entity player, Effect effect)
        {
            Console.WriteLine($"\n{player.Name}'s effect {effect.Name} stacked to {effect.RemainingStacks} stacks!");
        }
        */
    }
}
