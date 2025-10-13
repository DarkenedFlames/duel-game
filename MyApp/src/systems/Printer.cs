// ==================== Printer ====================
using System;
using System.Collections.Generic;
using System.Linq;

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
            ResourcesComponent? resources = player.GetComponent<ResourcesComponent>();
            StatsComponent? stats = player.GetComponent<StatsComponent>();

            if (resources != null && stats != null)
            {
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
            else Console.WriteLine($"{player.GetComponent<PlayerData>()?.Name} missing Stats or Resources component.");
        }

        // --- Status Effects ---
        public static void PrintEffects(Entity player)
        {
            IEnumerable<Entity>? effects = World.Instance.GetEntitiesWith<EffectData>()
                .Where(e => e.GetComponent<EffectData>()?.PlayerEntity == player);

            if (!effects.Any()) { Console.WriteLine("(No active effects)"); return; }

            foreach (Entity effect in effects)
            {
                EffectData? data = effect.GetComponent<EffectData>();
                int? duration = effect.GetComponent<EffectDuration>()?.Remaining;
                Console.WriteLine($"- {data?.Name ?? "Unknown"} (Duration: {duration} turns)");
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
        public static int? PrintItemMenu(Entity itemEntity)
        {
            ItemData? itemData = itemEntity.GetComponent<ItemData>();
            Entity? player = itemData?.PlayerEntity;
            PlayerData? playerData = player?.GetComponent<PlayerData>();
            Wearable? wearable = itemEntity.GetComponent<Wearable>();

            if (itemData == null || playerData == null)
            {
                Console.WriteLine("Invalid entity or missing components.");
                return null;
            }

            ClearAndHeader($"Item Menu: {playerData.Name} using {itemData.Name}");

            List<string>? actions = ["Remove"];

            if (itemData.PlayerEntity == player)
            {
                if (wearable != null)
                {
                    actions.Add(wearable.IsEquipped ? "Unequip" : "Equip");
                    if (itemData.Type == ItemType.Weapon && wearable.IsEquipped)
                        actions.Add("Use");
                }
                else if (itemData.Type == ItemType.Consumable)
                    actions.Add("Use");
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
                var name = targets[i].GetComponent<PlayerData>()?.Name ?? $"Target {i + 1}";
                Console.WriteLine($"{i + 1}. {name}");
            }
        }


        //================== Event Printers =================//
        public static void PrintTurnStartHeader(Entity player)
        {
            Console.Clear();
            Console.WriteLine($"-----{player.GetComponent<PlayerData>()?.Name}'s turn has began!-----");
        }
        public static void PrintTurnEndHeader(Entity player)
        {
            Console.WriteLine($"-----{player.GetComponent<PlayerData>()?.Name}'s turn has ended!-----");
        }


        public static void PrintEntityAdded(Entity entity)
        {
            if (entity.GetComponent<PlayerData>() != null)
            {
                Console.WriteLine($"\n{entity.GetComponent<PlayerData>()?.Name} has joined the game!");
            }
            if (entity.GetComponent<ItemData>() != null)
            {
                ItemData? itemData = entity.GetComponent<ItemData>();
                Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has picked up {itemData?.Name}!");
            }
            if (entity.GetComponent<EffectData>() != null)
            {
                EffectData? effectData = entity.GetComponent<EffectData>();
                EffectDuration? effectDuration = entity.GetComponent<EffectDuration>();
                if (effectDuration != null)
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has gained an effect: {effectData?.Name}! Remaining turns: {effectDuration.Remaining}");
                }
                else
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has gained an effect: {effectData?.Name}!");
                }
            }
        }
        public static void PrintEntityRemoved(Entity entity)
        {
            if (entity.GetComponent<PlayerData>() != null)
            {
                Console.WriteLine($"\n{entity.GetComponent<PlayerData>()?.Name} has died!");
            }
            if (entity.GetComponent<ItemData>() != null)
            {
                ItemData? itemData = entity.GetComponent<ItemData>();
                Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has lost {itemData?.Name}!");
            }
            if (entity.GetComponent<EffectData>() != null)
            {
                EffectData? effectData = entity.GetComponent<EffectData>();
                EffectDuration? effectDuration = entity.GetComponent<EffectDuration>();
                if (effectDuration != null)
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name}'s effect: {effectData?.Name} has expired!");
                }
                else
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has lost an effect: {effectData?.Name}!");
                }
            }
        }

        public static void PrintItemEquipped(Entity item)
        {
            ItemData? itemData = item.GetComponent<ItemData>();
            Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} equipped {itemData?.Name}!");
        }
        public static void PrintItemUnequipped(Entity item)
        {
            ItemData? itemData = item.GetComponent<ItemData>();
            Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} unequipped {itemData?.Name}!");
        }
        public static void PrintItemUsed(Entity item, Entity target)
        {
            ItemData? itemData = item.GetComponent<ItemData>();
            string? itemName = itemData?.Name;
            Entity? player = itemData?.PlayerEntity;
            bool usedOnSelf = player == target;
            string? playerName = player?.GetComponent<PlayerData>()?.Name;
            string? targetName = target.GetComponent<PlayerData>()?.Name;

            if (usedOnSelf)
            {
                Console.WriteLine($"\n{playerName} used {itemName} on themselves!");
            }
            else
            {
                Console.WriteLine($"\n{playerName} used {itemName} on {targetName}!");
            }

        }
        public static void PrintItemConsumed(Entity item)
        {
            ItemData? itemData = item.GetComponent<ItemData>();
            string? itemName = itemData?.Name;
            string? playerName = itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name;
            Console.WriteLine($"\n{playerName} consumed their {itemName}!");
        }

        public static void PrintInsufficientStamina(Entity item)
        {
            ItemData? itemData = item.GetComponent<ItemData>();
            Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} lacks stamina to use {itemData?.Name}.");
        }

        public static void PrintStatChanged(StatsComponent stats, string statName)
        {
            Console.WriteLine($"\n{stats.Owner.GetComponent<PlayerData>()?.Name}'s {statName} changed to {stats.Get(statName)}.");
        }
        public static void PrintResourceChanged(ResourcesComponent resources, string resourceName)
        {
            Console.WriteLine($"\n{resources.Owner.GetComponent<PlayerData>()?.Name}'s {resourceName} is now {resources.Get(resourceName)}.");
        }
        public static void PrintResourceDepleted(ResourcesComponent resources, string resourceName)
        {
            Console.WriteLine($"\n{resources.Owner.GetComponent<PlayerData>()?.Name}'s {resourceName} has been depleted!");
        }

        public static void PrintDamageDealt(Entity itemOrEffect, Entity target, int finalDamage)
        {
            ItemData? itemData = itemOrEffect.GetComponent<ItemData>();
            EffectData? effectData = itemOrEffect.GetComponent<EffectData>();

            bool isItem = itemData != null;
            Entity? user = isItem ? itemData?.PlayerEntity : effectData?.PlayerEntity;
            bool selfDamage = user == target;

            string? userName = user?.GetComponent<PlayerData>()?.Name;
            string? targetName = target.GetComponent<PlayerData>()?.Name;
            string? sourceName = isItem ? itemData?.Name : effectData?.Name;

            if (isItem)
            {
                if (selfDamage)
                {
                    Console.WriteLine($"\n{userName} damaged themselves with {sourceName} for {finalDamage} damage!");
                }
                else
                {
                    Console.WriteLine($"\n{userName} dealt {finalDamage} damage to {targetName} with {sourceName}!");
                }
            }
            else
            {
                Console.WriteLine($"\n{userName} took {finalDamage} damage from {sourceName}!");
            }
        }

        public static void PrintDodged(Entity itemOrEffect, Entity target)
        {
            ItemData? itemData = itemOrEffect.GetComponent<ItemData>();
            EffectData? effectData = itemOrEffect.GetComponent<EffectData>();
            string? targetName = target.GetComponent<PlayerData>()?.Name;

            if (itemData != null)
            {
                string itemName = itemData.Name;
                string? playerName = itemData.PlayerEntity.GetComponent<PlayerData>()?.Name;
                Console.WriteLine($"\n{playerName}'s attack with {itemName} was dodged by {targetName}!");

            }
            else if (effectData != null)
            {
                string effectName = effectData.Name;
                Console.WriteLine($"\n{targetName} dodged damage from their {effectName} effect!");
            }
            else
            {
                throw new NullReferenceException($"{itemOrEffect} was null when it should have been an <Entity>.");
            }
        }

        public static void PrintCritical(Entity itemOrEffect, Entity target)
        {
            ItemData? itemData = itemOrEffect.GetComponent<ItemData>();
            EffectData? effectData = itemOrEffect.GetComponent<EffectData>();
            string? targetName = target.GetComponent<PlayerData>()?.Name;

            if (itemData != null)
            {
                string? itemName = itemData.Name;
                string? playerName = itemData.PlayerEntity.GetComponent<PlayerData>()?.Name;
                Console.WriteLine($"\n{playerName}'s attack with {itemName} scored a critical hit on {targetName}!");

            }
            else if (effectData != null)
            {
                string effectName = effectData.Name;
                Console.WriteLine($"\n{targetName} was critically damaged by their {effectName} effect!");
            }
            else
            {
                throw new NullReferenceException($"{itemOrEffect} was null when it should have been an <Entity>.");
            }
        }

        public static void PrintPeered(List<String> targetItems, Entity target)
        {
            Console.WriteLine($"\nPeer activated! {target.GetComponent<PlayerData>()?.Name ?? "Enemy"}'s inventory:");
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
