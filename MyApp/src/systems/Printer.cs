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
            var resources = player.GetComponent<ResourcesComponent>();
            var stats = player.GetComponent<StatsComponent>();

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
            var effects = World.Instance.GetEntitiesWith<EffectData>()
                .Where(e => e.GetComponent<EffectData>()?.PlayerEntity == player);

            if (!effects.Any()) { Console.WriteLine("(No active effects)"); return; }

            foreach (var effect in effects)
            {
                var data = effect.GetComponent<EffectData>();
                var duration = effect.GetComponent<EffectDuration>()?.Remaining;
                Console.WriteLine($"- {data?.Name ?? "Unknown"} (Duration: {duration} turns)");
            }
        }

        // --- Items ---
        public static void PrintItemList(IEnumerable<Entity> items)
        {
            int idx = 1;
            foreach (var item in items)
            {
                var name = item.GetComponent<ItemData>()?.Name ?? "Unknown";
                Console.WriteLine($"{idx}. {name}");
                idx++;
            }
            if (idx == 1) Console.WriteLine("(No items found)");
        }

        // --- Equipment ---
        public static List<Entity> PrintEquipment(Entity player)
        {
            var equippedItems = World.Instance.GetEntitiesWith<Wearable>()
                .Where(e =>
                {
                    var wearable = e.GetComponent<Wearable>();
                    var itemData = e.GetComponent<ItemData>();
                    return wearable != null &&
                           itemData != null &&
                           wearable.IsEquipped &&
                           itemData.PlayerEntity == player;
                })
                .ToList();

            var slots = new List<(EquipType type, string label)>
            {
                (EquipType.Weapon, "Weapon"),
                (EquipType.Helmet, "Helmet"),
                (EquipType.Chestplate, "Chestplate"),
                (EquipType.Leggings, "Leggings"),
                (EquipType.Accessory, "Accessory")
            };

            var orderedEquipment = new List<Entity>();
            Console.WriteLine("Equipment:");
            for (int i = 0; i < slots.Count; i++)
            {
                var (type, label) = slots[i];
                var item = equippedItems.FirstOrDefault(e => e.GetComponent<Wearable>()?.EquipType == type);
                var itemName = item?.GetComponent<ItemData>()?.Name ?? "(empty)";
                Console.WriteLine($"{i + 1}. {label}: {itemName}");
                if (item != null) orderedEquipment.Add(item);
            }

            return orderedEquipment;
        }

        // --- Item Menu ---
        public static int? PrintItemMenu(Entity itemEntity)
        {
            var itemData = itemEntity.GetComponent<ItemData>();
            var player = itemData?.PlayerEntity;
            var playerData = player?.GetComponent<PlayerData>();
            var wearable = itemEntity.GetComponent<Wearable>();

            if (itemData == null || playerData == null)
            {
                Console.WriteLine("Invalid entity or missing components.");
                return null;
            }

            ClearAndHeader($"Item Menu: {playerData.Name} using {itemData.Name}");

            var actions = new List<string> { "Remove" };

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
                InputHandler.WaitForKey();
            }
            if (entity.GetComponent<ItemData>() != null)
            {
                var itemData = entity.GetComponent<ItemData>();
                Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has picked up {itemData?.Name}!");
                InputHandler.WaitForKey();
            }
            if (entity.GetComponent<EffectData>() != null)
            {
                var effectData = entity.GetComponent<EffectData>();
                var effectDuration = entity.GetComponent<EffectDuration>();
                if (effectDuration != null)
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has gained an effect: {effectData?.Name}! Remaining turns: {effectDuration.Remaining}");
                    InputHandler.WaitForKey();
                }
                else
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has gained an effect: {effectData?.Name}!");
                    InputHandler.WaitForKey();
                }
            }
        }
        public static void PrintEntityRemoved(Entity entity)
        {
            if (entity.GetComponent<PlayerData>() != null)
            {
                Console.WriteLine($"\n{entity.GetComponent<PlayerData>()?.Name} has died!");
                InputHandler.WaitForKey();
            }
            if (entity.GetComponent<ItemData>() != null)
            {
                var itemData = entity.GetComponent<ItemData>();
                Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has lost {itemData?.Name}!");
                InputHandler.WaitForKey();
            }
            if (entity.GetComponent<EffectData>() != null)
            {
                var effectData = entity.GetComponent<EffectData>();
                var effectDuration = entity.GetComponent<EffectDuration>();
                if (effectDuration != null)
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name}'s effect: {effectData?.Name} has expired!");
                    InputHandler.WaitForKey();
                }
                else
                {
                    Console.WriteLine($"\n{effectData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has lost an effect: {effectData?.Name}!");
                    InputHandler.WaitForKey();
                }
            }
        }

        public static void PrintItemEquipped(Entity item)
        {
            var itemData = item.GetComponent<ItemData>();
            Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} equipped {itemData?.Name}!");
            InputHandler.WaitForKey();
        }
        public static void PrintItemUnequipped(Entity item)
        {
            var itemData = item.GetComponent<ItemData>();
            Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} unequipped {itemData?.Name}!");
            InputHandler.WaitForKey();
        }
        public static void PrintItemUsed(Entity item, Entity target)
        {
            var itemData = item.GetComponent<ItemData>();
            var itemName = itemData?.Name;
            var player = itemData?.PlayerEntity;
            bool usedOnSelf = player == target;
            var playerName = player?.GetComponent<PlayerData>()?.Name;
            var targetName = target.GetComponent<PlayerData>()?.Name;

            if (!usedOnSelf)
            {
                Console.WriteLine($"\n{playerName} used {itemName} on themselves!");
                InputHandler.WaitForKey();
            }
            else
            {
                Console.WriteLine($"\n{playerName} used {itemName} on {targetName}!");
                InputHandler.WaitForKey();
            }

        }
        public static void PrintItemConsumed(Entity item)
        {
            var itemData = item.GetComponent<ItemData>();
            var itemName = itemData?.Name;
            var playerName = itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name;
            Console.WriteLine($"\n{playerName} consumed their {itemName}!");
            InputHandler.WaitForKey();
        }

        public static void PrintInsufficientStamina(Entity item)
        {
            var itemData = item.GetComponent<ItemData>();
            Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} lacks stamina to use {itemData?.Name}.");
            InputHandler.WaitForKey();
        }

        public static void PrintStatChanged(StatsComponent stats, string statName)
        {
            Console.WriteLine($"\n{stats.Owner.GetComponent<PlayerData>()?.Name}'s {statName} changed to {stats.Get(statName)}.");
            InputHandler.WaitForKey();
        }
        public static void PrintResourceChanged(ResourcesComponent resources, string resourceName)
        {
            Console.WriteLine($"\n{resources.Owner.GetComponent<PlayerData>()?.Name}'s {resourceName} is now {resources.Get(resourceName)}.");
            InputHandler.WaitForKey();
        }
        public static void PrintResourceDepleted(ResourcesComponent resources, string resourceName)
        {
            Console.WriteLine($"\n{resources.Owner.GetComponent<PlayerData>()?.Name}'s {resourceName} has been depleted!");
            InputHandler.WaitForKey();
        }

        public static void PrintDamageDealt(Entity itemOrEffect, Entity target, int finalDamage)
        {
            var itemData = itemOrEffect.GetComponent<ItemData>();
            var effectData = itemOrEffect.GetComponent<EffectData>();

            bool isItem = itemData != null;
            var user = isItem ? itemData?.PlayerEntity : effectData?.PlayerEntity;
            bool selfDamage = user == target;

            string? userName = user?.GetComponent<PlayerData>()?.Name;
            string? targetName = target.GetComponent<PlayerData>()?.Name;
            string? sourceName = isItem ? itemData?.Name : effectData?.Name;

            if (isItem)
            {
                if (selfDamage)
                {
                    Console.WriteLine($"\n{userName} damaged themselves with {sourceName} for {finalDamage} damage!");
                    InputHandler.WaitForKey();
                }
                else
                {
                    Console.WriteLine($"\n{userName} dealt {finalDamage} damage to {targetName} with {sourceName}!");
                    InputHandler.WaitForKey();
                }
            }
            else
            {
                Console.WriteLine($"\n{userName} took {finalDamage} damage from {sourceName}!");
                InputHandler.WaitForKey();
            }
        }

        public static void PrintDodged(Entity itemOrEffect, Entity target)
        {
            var itemData = itemOrEffect.GetComponent<ItemData>();
            var effectData = itemOrEffect.GetComponent<EffectData>();
            var targetName = target.GetComponent<PlayerData>()?.Name;

            if (itemData != null)
            {
                var itemName = itemData.Name;
                var playerName = itemData.PlayerEntity.GetComponent<PlayerData>()?.Name;
                Console.WriteLine($"\n{playerName}'s attack with {itemName} was dodged by {targetName}!");
                InputHandler.WaitForKey();

            }
            else if (effectData != null)
            {
                var effectName = effectData.Name;
                Console.WriteLine($"\n{targetName} dodged damage from their {effectName} effect!");
                InputHandler.WaitForKey();
            }
            else
            {
                throw new NullReferenceException($"{itemOrEffect} was null when it should have been an <Entity>.");
            }
        }

        public static void PrintCritical(Entity itemOrEffect, Entity target)
        {
            var itemData = itemOrEffect.GetComponent<ItemData>();
            var effectData = itemOrEffect.GetComponent<EffectData>();
            var targetName = target.GetComponent<PlayerData>()?.Name;

            if (itemData != null)
            {
                var itemName = itemData.Name;
                var playerName = itemData.PlayerEntity.GetComponent<PlayerData>()?.Name;
                Console.WriteLine($"\n{playerName}'s attack with {itemName} scored a critical hit on {targetName}!");
                InputHandler.WaitForKey();

            }
            else if (effectData != null)
            {
                var effectName = effectData.Name;
                Console.WriteLine($"\n{targetName} was critically damaged by their {effectName} effect!");
                InputHandler.WaitForKey();
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
