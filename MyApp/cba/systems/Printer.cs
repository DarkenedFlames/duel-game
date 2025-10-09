using System;
using System.Collections.Generic;
using System.Linq;
using MyApp;

namespace CBA
{
    public class Printer
    {
        private static void AnyKey()
        {
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey(true);
        }
        private static void ClearAndHeader(string header)
        {
            Console.Clear();
            Console.WriteLine(new string('-', header.Length));
            Console.WriteLine(header);
            Console.WriteLine(new string('-', header.Length));
        }
        public static bool PrintMainMenu(Entity player)
        {
            var playerData = player.GetComponent<PlayerData>();
            var playerName = playerData?.Name;
            ClearAndHeader($"Main Menu: {playerName}'s Turn");
            Console.WriteLine("[1] Stats [2] Inventory [3] Equipment [4] Status [5] End Turn");
            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ClearAndHeader($"{playerName}'s Stats");
                    Console.WriteLine();
                    StatMenu(player);
                    break;
                case "2":
                    ClearAndHeader($"{playerName}'s Inventory");
                    Console.WriteLine("\nChoose an Item (Enter to exit):");
                    InventoryMenu(player);
                    break;
                case "3":
                    ClearAndHeader($"{playerName}'s Equipment");
                    Console.WriteLine("\nChoose Equipment:");
                    EquipmentMenu(player);
                    break;
                case "4":
                    ClearAndHeader($"{playerName}'s Status");
                    StatusMenu(player);
                    break;
                case "5": return true; // signal end turn
                default: Console.WriteLine("Invalid choice. Please try again."); break;
            }
            return false; // stay in menu
        }
        private static void StatusMenu(Entity player)
        {
            var allEffects = World.Instance.GetEntitiesWith<EffectData>().ToList();
            List<Entity> playerEffects = [];

            foreach (Entity effect in allEffects)
            {
                var effectData = effect.GetComponent<EffectData>();

                if (effectData?.PlayerEntity == player)
                {
                    playerEffects.Add(effect);
                }
            }

            if (playerEffects.Count == 0)
            {
                Console.WriteLine("(No active effects)");
                AnyKey();
                return;
            }
            foreach (var effect in playerEffects)
            {
                var effectName = effect.GetComponent<EffectData>()?.Name;
                var remaining = effect.GetComponent<EffectDuration>()?.Remaining;
                Console.WriteLine($"- {effectName} (Duration: {remaining} turns)");
            }
            AnyKey();
        }
        private static void StatMenu(Entity player)
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
            else
            {
                throw new Exception($"{player.GetComponent<PlayerData>()?.Name} has no StatsComponent or no ResourcesComponent.");
            }
            AnyKey();
        }
        private static void InventoryMenu(Entity player)
        {
            var allItems = World.Instance.GetEntitiesWith<ItemData>().ToList();
            List<Entity> playerItems = [];

            foreach (Entity item in allItems)
            {
                var itemData = item.GetComponent<ItemData>();

                if (itemData?.PlayerEntity == player)
                {
                    playerItems.Add(item);
                }
            }
            int idx = 1;
            foreach (var item in playerItems)
            {
                Console.WriteLine($"{idx}. {item.GetComponent<ItemData>()?.Name}");
                idx++;
            }

            if (idx == 1)
            {
                Console.WriteLine("(Inventory is empty)");
                AnyKey();
            }

            string? choice = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(choice)) return;

            if (int.TryParse(choice, out int index) && index > 0 && index < playerItems.Count + 1)
            {
                PrintItemMenu(player, playerItems[index - 1]);
            }
            else
            {
                Console.WriteLine("Invalid choice.");
                AnyKey();
            }
        }
        private static void EquipmentMenu(Entity player)
        {
            var world = World.Instance;

            // All equipped items that belong to this player
            var equippedItems = world.GetEntitiesWith<Wearable>()
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

            var slotChoices = new List<(int number, EquipType type, string label)>
            {
                (1, EquipType.Weapon, "Weapon"),
                (2, EquipType.Helmet, "Helmet"),
                (3, EquipType.Chestplate, "Chestplate"),
                (4, EquipType.Leggings, "Leggings"),
                (5, EquipType.Accessory, "Accessory")
            };

            // Display all “slots” with what’s currently equipped
            foreach (var (number, type, label) in slotChoices)
            {
                var equippedEntity = equippedItems.FirstOrDefault(e =>
                    e.GetComponent<Wearable>()?.EquipType == type);

                var itemName = equippedEntity?.GetComponent<ItemData>()?.Name ?? "(empty)";
                Console.WriteLine($"{number}. {label}: {itemName}");
            }

            Console.WriteLine("Press Enter to return to the main menu...");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                return;

            if (!int.TryParse(input, out int choice) || choice < 1 || choice > slotChoices.Count)
            {
                Console.WriteLine("Invalid choice.");
                AnyKey();
                return;
            }

            var selectedType = slotChoices[choice - 1].type;

            // Find the equipped item of that type
            var selectedItem = equippedItems.FirstOrDefault(e =>
                e.GetComponent<Wearable>()?.EquipType == selectedType);

            if (selectedItem != null)
            {
                PrintItemMenu(player, selectedItem);
            }
            else
            {
                Console.WriteLine("No item equipped in that slot.");
                AnyKey();
            }
        }


        public static void PrintItemMenu(Entity player, Entity selectedItem)
        {
            var playerData = player.GetComponent<PlayerData>();
            var itemData = selectedItem.GetComponent<ItemData>();
            var wearable = selectedItem.GetComponent<Wearable>();
            var allPlayers = World.Instance.GetEntitiesWith<PlayerData>().ToList();

            if (playerData == null || itemData == null)
            {
                Console.WriteLine("Invalid entity or missing components.");
                return;
            }

            // === Header ===
            ClearAndHeader($"Item Menu: {playerData.Name} using {itemData.Name}");

            // === Determine available actions ===
            List<string> actions = new() { "Remove Item" };

            // If the player owns the item
            if (itemData.PlayerEntity == player)
            {
                // Has Wearable component
                if (wearable != null)
                {
                    if (!wearable.IsEquipped)
                        actions.Add("Equip Item");
                    else
                        actions.Add("Unequip Item");

                    // Weapons can only be used if equipped
                    if (itemData.Type == ItemType.Weapon && wearable.IsEquipped)
                        actions.Add("Use Item");
                }
                else if (itemData.Type == ItemType.Consumable)
                {
                    // Consumables can always be used
                    actions.Add("Use Item");
                }
            }

            // === Display available actions ===
            Console.WriteLine("Available Actions:");
            for (int i = 0; i < actions.Count; i++)
                Console.WriteLine($"{i + 1}. {actions[i]}");

            Console.WriteLine("Enter a number to select an action, or press Enter to return.");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return;

            if (!int.TryParse(input, out int choice) || choice < 1 || choice > actions.Count)
            {
                Console.WriteLine("Invalid choice.");
                AnyKey();
                return;
            }

            string selectedAction = actions[choice - 1].ToLower();

            // === Perform action ===
            switch (selectedAction)
            {
                case "remove item":
                    // Probably just delete or mark removed in world
                    World.Instance.RemoveEntity(selectedItem);
                    Console.WriteLine($"{itemData.Name} removed from world.");
                    break;

                case "equip item":
                    if (wearable != null && !wearable.IsEquipped)
                    {
                        wearable.TryEquip();
                        Console.WriteLine($"{playerData.Name} equipped {itemData.Name}.");
                    }
                    else
                        Console.WriteLine("Cannot equip this item.");
                    break;

                case "unequip item":
                    if (wearable != null && wearable.IsEquipped)
                    {
                        wearable.TryUnequip();
                        Console.WriteLine($"{playerData.Name} unequipped {itemData.Name}.");
                    }
                    else
                        Console.WriteLine("Cannot unequip this item.");
                    break;

                case "use item":
                    if (itemData.Type == ItemType.Consumable ||
                        (itemData.Type == ItemType.Weapon && wearable?.IsEquipped == true))
                    {
                        // === Choose target ===
                        ClearAndHeader($"Choose target for {itemData.Name}:");
                        for (int i = 0; i < allPlayers.Count; i++)
                        {
                            var pData = allPlayers[i].GetComponent<PlayerData>();
                            Console.WriteLine($"{i + 1}. {pData?.Name ?? "Unknown"}");
                        }

                        string? targetInput = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(targetInput)) return;

                        if (!int.TryParse(targetInput, out int targetIndex) ||
                            targetIndex < 1 || targetIndex > allPlayers.Count)
                        {
                            Console.WriteLine("Invalid choice.");
                            AnyKey();
                            return;
                        }

                        var target = allPlayers[targetIndex - 1];
                        var targetData = target.GetComponent<PlayerData>();

                        Console.WriteLine($"{playerData.Name} used {itemData.Name} on {targetData?.Name ?? "Unknown"}!");

                        selectedItem.GetComponent<Usable>()?.TryUse(target);
                    }
                    else
                    {
                        Console.WriteLine("Cannot use this item right now.");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid action.");
                    break;
            }

            AnyKey();
        }




        //================== Event Printers =================//

        public static void PrintEntityAdded(Entity entity)
        {
            if (entity.GetComponent<PlayerData>() != null)
            {
                Console.WriteLine($"\n{entity.GetComponent<PlayerData>()?.Name} has joined the game!");
            }
            if (entity.GetComponent<ItemData>() != null)
            {
                var itemData = entity.GetComponent<ItemData>();
                Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has picked up {itemData?.Name}!");
            }
            if (entity.GetComponent<EffectData>() != null)
            {
                var effectData = entity.GetComponent<EffectData>();
                var effectDuration = entity.GetComponent<EffectDuration>();
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
                var itemData = entity.GetComponent<ItemData>();
                Console.WriteLine($"\n{itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name} has lost {itemData?.Name}!");
            }
            if (entity.GetComponent<EffectData>() != null)
            {
                var effectData = entity.GetComponent<EffectData>();
                var effectDuration = entity.GetComponent<EffectDuration>();
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
            var itemData = item.GetComponent<ItemData>();
            var playerName = itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name;
            var itemName = itemData?.Name;
            Console.WriteLine($"\n{playerName} equipped {itemName}!");
        }
        public static void PrintItemUnequipped(Entity item)
        {
            var itemData = item.GetComponent<ItemData>();
            var playerName = itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name;
            var itemName = itemData?.Name;
            Console.WriteLine($"\n{playerName} unequipped {itemName}!");
        }
        public static void PrintItemUsed(Entity item, Entity target)
        {
            var itemData = item.GetComponent<ItemData>();
            var playerName = itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name;
            var itemName = itemData?.Name;
            var targetName = target.GetComponent<PlayerData>()?.Name;
            Console.WriteLine($"\n{playerName} used {itemName} on {targetName}!");
        }

        public static void PrintInsufficientStamina(Entity item)
        {
            var itemData = item.GetComponent<ItemData>();
            var playerName = itemData?.PlayerEntity.GetComponent<PlayerData>()?.Name;
            var itemName = itemData?.Name;
            Console.WriteLine($"\n{playerName} lacks stamina to use {itemName}.");
        }

        public static void PrintStatChanged(StatsComponent stats, string statName)
        {
            var playerName = stats.Owner.GetComponent<PlayerData>();
            Console.WriteLine($"\n{playerName}'s {statName} changed to {stats.Get(statName)}.");
        }
        public static void PrintResourceChanged(ResourcesComponent resources, string resourceName)
        {
            var playerName = resources.Owner.GetComponent<PlayerData>();
            Console.WriteLine($"\n{playerName}'s {resourceName} is now {resources.Get(resourceName)}.");
        }
        public static void PrintResourceDepleted(ResourcesComponent resources, string resourceName)
        {
            var playerName = resources.Owner.GetComponent<PlayerData>();
            Console.WriteLine($"\n{playerName}'s {resourceName} has been depleted!");
        }



        /*
        public static void PrintEffectStacked(Entity player, Effect effect)
        {
            Console.WriteLine($"\n{player.Name}'s effect {effect.Name} stacked to {effect.RemainingStacks} stacks!");
        }
        */
    }
}