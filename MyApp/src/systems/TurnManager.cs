using System;
using System.Linq;
using System.Collections.Generic;

namespace CBA
{
    public class TurnManager
    {
        private int _turnIndex = 0;

        public void StartGameLoop()
        {
            var turnEntities = World.Instance.GetEntitiesWith<TakesTurns>().ToList();

            if (turnEntities.Count == 0)
            {
                Console.WriteLine("No entities have TakesTurns component.");
                return;
            }

            while (turnEntities.Count > 1)
            {
                // Refresh entities (remove dead/deleted)
                turnEntities = World.Instance.GetEntitiesWith<TakesTurns>().ToList();
                if (turnEntities.Count == 0) break;

                // Sort by TurnOrder
                var ordered = turnEntities
                    .Select(e => e.GetComponent<TakesTurns>())
                    .OrderBy(t => t!.TurnOrder)
                    .ToList();

                // Clamp turn index
                if (_turnIndex >= ordered.Count) _turnIndex = 0;

                var currentTurn = ordered[_turnIndex];
                if (currentTurn == null || !currentTurn.IsActive)
                {
                    _turnIndex++;
                    continue;
                }

                var player = currentTurn.Owner;
                var playerData = player.GetComponent<PlayerData>();

                // START TURN
                currentTurn.StartTurn();

                // --- Player Menu Loop ---
                bool endTurn = false;
                while (!endTurn)
                {
                    Printer.ClearAndHeader($"Main Menu: {playerData?.Name}'s Turn");
                    Console.WriteLine("[1] Stats [2] Inventory [3] Equipment [4] Status [5] End Turn");
                    Console.Write("Choose an option: ");
                    string? choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1": // Stats
                            Printer.ClearAndHeader($"{playerData?.Name}'s Stats");
                            Printer.PrintStats(player);
                            WaitForKey();
                            break;

                        case "2": // Inventory
                            Printer.ClearAndHeader($"{playerData?.Name}'s Inventory");
                            var items = World.Instance.GetEntitiesWith<ItemData>()
                                .Where(i => i.GetComponent<ItemData>()?.PlayerEntity == player)
                                .ToList();

                            Printer.PrintItemList(items);

                            Console.WriteLine("\nEnter item number to open, or press Enter to go back:");
                            string? itemInput = Console.ReadLine();
                            if (int.TryParse(itemInput, out int idx) && idx > 0 && idx <= items.Count)
                            {
                                var selectedItem = items[idx - 1];
                                Printer.ClearAndHeader($"Item Menu: {playerData?.Name} using {selectedItem.GetComponent<ItemData>()?.Name}");
                                string? action = Printer.PrintItemMenu(selectedItem);
                                HandleItemAction(selectedItem, action);
                                WaitForKey();
                            }
                            break;

                        case "3": // Equipment
                            Printer.ClearAndHeader($"{playerData?.Name}'s Equipment");
                            Printer.PrintEquipment(player);
                            WaitForKey();
                            break;

                        case "4": // Status
                            Printer.ClearAndHeader($"{playerData?.Name}'s Status");
                            Printer.PrintEffects(player);
                            WaitForKey();
                            break;

                        case "5": // End Turn
                            endTurn = true;
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            WaitForKey();
                            break;
                    }
                }

                // END TURN
                currentTurn.EndTurn();

                // Advance turn
                _turnIndex++;
            }

            Console.WriteLine("\nGame Over.");
        }

        public static void WaitForKey()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private void HandleItemAction(Entity itemEntity, string? action)
        {
            if (string.IsNullOrWhiteSpace(action)) return;

            var itemData = itemEntity.GetComponent<ItemData>();
            var wearable = itemEntity.GetComponent<Wearable>();
            var player = itemData?.PlayerEntity;
            var playerData = player?.GetComponent<PlayerData>();

            switch (action.ToLower())
            {
                case "remove item":
                    World.Instance.RemoveEntity(itemEntity);
                    Console.WriteLine($"{itemData?.Name} removed from world.");
                    break;

                case "equip item":
                    if (wearable != null && !wearable.IsEquipped)
                    {
                        wearable.TryEquip();
                        Console.WriteLine($"{playerData?.Name} equipped {itemData?.Name}.");
                    }
                    else
                        Console.WriteLine("Cannot equip this item.");
                    break;

                case "unequip item":
                    if (wearable != null && wearable.IsEquipped)
                    {
                        wearable.TryUnequip();
                        Console.WriteLine($"{playerData?.Name} unequipped {itemData?.Name}.");
                    }
                    else
                        Console.WriteLine("Cannot unequip this item.");
                    break;

                case "use item":
                    if (itemData != null && 
                        (itemData.Type == ItemType.Consumable || (itemData.Type == ItemType.Weapon && wearable?.IsEquipped == true)))
                    {
                        // Determine target
                        var allPlayers = World.Instance.GetEntitiesWith<PlayerData>().ToList();
                        Console.WriteLine($"Choose target for {itemData.Name}:");
                        for (int i = 0; i < allPlayers.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {allPlayers[i].GetComponent<PlayerData>()?.Name ?? "Unknown"}");
                        }

                        string? targetInput = Console.ReadLine();
                        if (int.TryParse(targetInput, out int targetIndex) && targetIndex >= 1 && targetIndex <= allPlayers.Count)
                        {
                            var target = allPlayers[targetIndex - 1];
                            Console.WriteLine($"{playerData?.Name} used {itemData.Name} on {target.GetComponent<PlayerData>()?.Name ?? "Unknown"}!");
                            itemEntity.GetComponent<Usable>()?.TryUse(target);
                        }
                        else
                        {
                            Console.WriteLine("Invalid target selection.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cannot use this item right now.");
                    }
                    break;

                default:
                    Console.WriteLine("Unknown action.");
                    break;
            }
        }
    }
}