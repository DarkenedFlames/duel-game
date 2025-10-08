using System;
using System.Collections.Generic;

namespace MyApp
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
        public static bool PrintMainMenu(Player player)
        {
            ClearAndHeader($"Main Menu: {player.Name}'s Turn");
            Console.WriteLine("[1] Stats [2] Inventory [3] Equipment [4] Status [5] End Turn");
            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ClearAndHeader($"{player.Name}'s Stats");
                    Console.WriteLine();
                    StatMenu(player);
                    break;
                case "2":
                    ClearAndHeader($"{player.Name}'s Inventory");
                    Console.WriteLine("\nChoose an Item (Enter to exit):");
                    InventoryMenu(player);
                    break;
                case "3":
                    ClearAndHeader($"{player.Name}'s Equipment");
                    Console.WriteLine("\nChoose Equipment:");
                    EquipmentMenu(player);
                    break;
                case "4":
                    ClearAndHeader($"{player.Name}'s Status");
                    StatusMenu(player);
                    break;
                case "5": return true; // signal end turn
                default: Console.WriteLine("Invalid choice. Please try again."); break;
            }
            return false; // stay in menu
        }
        private static void StatusMenu(Player player)
        {
            if (player.ActiveEffects.Effects.Count == 0)
            {
                Console.WriteLine("(No active effects)");
                AnyKey();
                return;
            }
            foreach (var effect in player.ActiveEffects.Effects)
            {
                Console.WriteLine($"- {effect.Name} (Duration: {effect.RemainingDuration} turns)");
            }
            AnyKey();
        }
        private static void StatMenu(Player player)
        {
            Console.WriteLine($"Health: {player.Resources.Get("Health")} / {player.Stats.Get("MaximumHealth")}");
            Console.WriteLine($"Stamina: {player.Resources.Get("Stamina")} / {player.Stats.Get("MaximumStamina")}");
            Console.WriteLine($"Armor: {player.Stats.Get("Armor")}");
            Console.WriteLine($"Shield: {player.Stats.Get("Shield")}");
            Console.WriteLine($"Critical: {player.Stats.Get("Critical")}");
            Console.WriteLine($"Dodge: {player.Stats.Get("Dodge")}");
            Console.WriteLine($"Peer: {player.Stats.Get("Peer")}");
            Console.WriteLine($"Luck: {player.Stats.Get("Luck")}");
            Console.WriteLine($"Healing Modifier: {player.Resources.Values["Health"].RestoreMult:P}");
            Console.WriteLine($"Stimming Modifier: {player.Resources.Values["Stamina"].RestoreMult:P}");
            AnyKey();
        }
        private static void InventoryMenu(Player player)
        {
            int idx = 1;
            foreach (var item in player.Inventory.Items)
            {
                Console.WriteLine($"{idx}. {item.Name}");
                idx++;
            }

            if (idx == 1)
            {
                Console.WriteLine("(Inventory is empty)");
                AnyKey();
            }

            string? choice = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(choice)) return;

            if (int.TryParse(choice, out int index) && index > 0 && index < player.Inventory.Items.Count + 1)
            {
                PrintItemMenu(player, player.Inventory.Items[index - 1]);
            }
            else
            {
                Console.WriteLine("Invalid choice.");
                AnyKey();
            }
        }
        private static void EquipmentMenu(Player player)
        {
            var slotChoices = new List<(int number, ItemType type, string label)>
            {
                (1, ItemType.Weapon, "Weapon"),
                (2, ItemType.Helmet, "Helmet"),
                (3, ItemType.Chestplate, "Chestplate"),
                (4, ItemType.Leggings, "Leggings"),
                (5, ItemType.Accessory, "Accessory")
            };

            // Print slots with numbers
            foreach (var (number, type, label) in slotChoices)
            {
                if (player.Equipment.Slots.TryGetValue(type, out var item) && item != null)
                    Console.WriteLine($"{number}. {label}: {item.Name}");
                else
                    Console.WriteLine($"{number}. {label}: (empty)");
            }

            Console.WriteLine("Press Enter to return to the main menu...");
            string? input = Console.ReadLine();

            // Cancel if Enter pressed
            if (string.IsNullOrWhiteSpace(input))
                return;

            // Parse numeric selection
            if (!int.TryParse(input, out int choice) || choice < 1 || choice > slotChoices.Count)
            {
                Console.WriteLine("Invalid choice.");
                AnyKey();
                return;
            }

            // Get selected item type
            var selectedType = slotChoices[choice - 1].type;

            if (player.Equipment.Slots.TryGetValue(selectedType, out var selectedItem) && selectedItem != null)
            {
                PrintItemMenu(player, selectedItem);
            }
            else
            {
                Console.WriteLine("No item equipped in that slot.");
                AnyKey();
            }
        }
        private static void PrintItemMenu(Player player, Item selectedItem)
        {
            ClearAndHeader($"Item Menu: {player.Name} using {selectedItem.Name}");

            // Build available actions based on item type and location
            var actions = new List<string> { "Remove Item" };
            List<Player> players = TurnManager.players;

            if (player.Inventory.Contains(selectedItem))
            {
                if (selectedItem.IsEquipment())
                    actions.Add("Equip Item");
                else if (selectedItem.Type == ItemType.Consumable)
                    actions.Add("Use Item");
            }
            else if (player.Equipment.Slots.Any(kvp => kvp.Value == selectedItem))
            {
                actions.Add("Unequip Item");
                if (selectedItem.Type == ItemType.Weapon)
                    actions.Add("Use Item");
            }

            // Display item info
            Console.WriteLine("Available Actions:");
            for (int i = 0; i < actions.Count; i++)
                Console.WriteLine($"{i + 1}. {actions[i]}");

            // Prompt user
            Console.WriteLine("Enter a number to select an action, or press Enter to return to the main menu.");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return;

            if (!int.TryParse(input, out int actionIndex) || actionIndex < 1 || actionIndex > actions.Count)
            {
                Console.WriteLine("Invalid action.");
                AnyKey();
                return;
            }

            string selectedAction = actions[actionIndex - 1].ToLower();

            switch (selectedAction)
            {
                case "remove item":
                    player.Inventory.RemoveItem(selectedItem);
                    break;

                case "equip item":
                    if (actions.Contains("Equip Item"))
                        player.Inventory.TryEquip(selectedItem);
                    else
                        Console.WriteLine("Cannot equip this item.");
                    break;

                case "unequip item":
                    if (actions.Contains("Unequip Item"))
                    {
                        foreach (var kvp in player.Equipment.Slots)
                        {
                            if (kvp.Value == selectedItem)
                            {
                                player.Equipment.Unequip(kvp.Key);
                                break;
                            }
                        }
                    }
                    else
                        Console.WriteLine("Cannot unequip this item.");
                    break;

                case "use item":
                    if (!actions.Contains("Use Item"))
                    {
                        Console.WriteLine("Cannot use this item.");
                        break;
                    }

                    // Prompt player to select a target
                    ClearAndHeader($"Item Menu: {player.Name} using {selectedItem.Name}");
                    Console.WriteLine("\nChoose a target (or press Enter to cancel):");
                    for (int i = 0; i < players.Count; i++)
                        Console.WriteLine($"{i + 1}. {players[i].Name}");

                    string? targetInput = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(targetInput)) return; // cancel

                    if (!int.TryParse(targetInput, out int targetIndex) || targetIndex < 1 || targetIndex > players.Count)
                    {
                        Console.WriteLine("Invalid choice.");
                        AnyKey();
                        return;
                    }

                    Player target = players[targetIndex - 1];
                    selectedItem.TryUse(target);

                    break;


                default:
                    Console.WriteLine("Invalid action.");
                    break;
            }

            AnyKey(); // Pause before returning to menu
        }



        //================== Event Printers =================//
        public static void PrintItemEquipped(Player player, Item item)
        {
            Console.WriteLine($"\n{player.Name} equipped {item.Name}!");
        }
        public static void PrintItemUnequipped(Player player, Item item)
        {
            Console.WriteLine($"\n{player.Name} unequipped {item.Name}!");
        }
        public static void PrintItemUsed(Player player, Item item, Player target)
        {
            Console.WriteLine($"\n{player.Name} used {item.Name} on {target.Name}!");
        }
        public static void PrintItemReceived(Player player, Item item)
        {
            Console.WriteLine($"\n{player.Name} received {item.Name}!");
        }
        public static void PrintItemLost(Player player, Item item)
        {
            Console.WriteLine($"\n{player.Name} lost {item.Name}!");
        }

        public static void PrintInsufficientStamina(Player player, Item item)
        {
            Console.WriteLine($"\n{player.Name} lacks stamina to use {item.Name}.");
        }
        public static void PrintEffectAdded(Player player, Effect effect)
        {
            Console.WriteLine($"\n{player.Name} gained effect: {effect.Name} (Duration: {effect.RemainingDuration} turns)");
        }
        public static void PrintEffectStacked(Player player, Effect effect)
        {
            Console.WriteLine($"\n{player.Name}'s effect {effect.Name} stacked to {effect.RemainingStacks} stacks!");
        }
        public static void PrintEffectRemoved(Player player, Effect effect)
        {
            Console.WriteLine($"\n{player.Name}'s effect {effect.Name} has expired.");
        }
        public static void PrintEffectTicked(Player player, Effect effect)
        {
            Console.WriteLine($"\n{player.Name}'s effect {effect.Name} ticks. (Duration left: {effect.RemainingDuration} turns)");
        }
        public static void PrintStatChanged(Player player, string statName, int newValue)
        {
            Console.WriteLine($"\n{player.Name}'s {statName} changed to {newValue}.");
        }
        public static void PrintStatModifierChanged(Player player, string statName, float factor)
        {
            Console.WriteLine($"\n{player.Name}'s {statName} modifier changed to {factor:P}.");
        }
        public static void PrintResourceChanged(Player player, string resourceName, int newValue)
        {
            Console.WriteLine($"\n{player.Name}'s {resourceName} is now {newValue}.");
        }
        public static void PrintResourceDepleted(Player player, string resourceName)
        {
            Console.WriteLine($"\n{player.Name}'s {resourceName} has been depleted!");
        }

    }
}