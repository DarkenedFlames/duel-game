// ==================== TurnManager ====================
using System;
using System.Collections.Generic;
using System.Linq;

namespace CBA
{
    public class TurnManager
    {
        public void StartGameLoop()
        {
            var turnEntities = World.Instance.GetEntitiesWith<TakesTurns>().ToList();
            if (turnEntities.Count == 0)
            {
                Printer.PrintMessage("No entities have TakesTurns component.");
                return;
            }

            while (true) // main game loop
            {
                // Order turns for this round
                var orderedTurns = turnEntities
                    .Select(e => e.GetComponent<TakesTurns>())
                    .OrderBy(t => t!.TurnOrder)
                    .ToList();

                foreach (var currentTurn in orderedTurns)
                {
                    if (currentTurn == null) continue;

                    var player = currentTurn.Owner;
                    var playerData = player.GetComponent<PlayerData>();

                    currentTurn.StartTurn(); // fires OnTurnStart

                    HandlePlayerMenu(player, playerData);

                    currentTurn.EndTurn(); // fires OnTurnEnd
                }

                // Check game over: if <=1 alive player remains
                var alivePlayers = turnEntities
                    .Where(e => e.GetComponent<PlayerData>() != null)
                    .ToList();

                if (alivePlayers.Count <= 1)
                    break;
            }

            Printer.PrintMessage("\nGame Over.");
        }



        private void HandlePlayerMenu(Entity player, PlayerData? playerData)
        {
            bool endTurn = false;

            while (!endTurn)
            {
                Printer.ClearAndHeader($"Main Menu: {playerData?.Name}'s Turn");
                Printer.PrintMenu(new List<string> { "Stats", "Inventory", "Equipment", "Status", "End Turn" });
                int choice = InputHandler.GetNumberInput(1, 5);

                switch (choice)
                {
                    case 1: // Stats
                        Printer.ClearAndHeader($"{playerData?.Name}'s Stats");
                        Printer.PrintStats(player);
                        InputHandler.WaitForKey();
                        break;

                    case 2: // Inventory
                        HandleInventoryMenu(player, playerData);
                        break;

                    case 3: // Equipment
                        HandleEquipmentMenu(player, playerData);
                        break;

                    case 4: // Status
                        Printer.ClearAndHeader($"{playerData?.Name}'s Status");
                        Printer.PrintEffects(player);
                        InputHandler.WaitForKey();
                        break;

                    case 5: // End Turn
                        endTurn = true;
                        break;

                    default:
                        Printer.PrintMessage("Invalid choice. Try again.");
                        InputHandler.WaitForKey();
                        break;
                }
            }
        }

        private void HandleInventoryMenu(Entity player, PlayerData? playerData)
        {
            Printer.ClearAndHeader($"{playerData?.Name}'s Inventory");
            var items = World.Instance.GetEntitiesWith<ItemData>()
                .Where(i => i.GetComponent<ItemData>()?.PlayerEntity == player)
                .ToList();

            Printer.PrintItemList(items);
            int itemIdx = InputHandler.GetNumberInput(0, items.Count, "Enter item number to open, or 0 to go back:");

            if (itemIdx > 0 && itemIdx <= items.Count)
            {
                var selectedItem = items[itemIdx - 1];
                int? action = Printer.PrintItemMenu(selectedItem);
                HandleItemAction(selectedItem, action);
                InputHandler.WaitForKey();
            }
        }

        private void HandleEquipmentMenu(Entity player, PlayerData? playerData)
        {
            Printer.ClearAndHeader($"{playerData?.Name}'s Equipment");
            var equippedItems = Printer.PrintEquipment(player);

            if (!equippedItems.Any())
            {
                InputHandler.WaitForKey();
                return;
            }

            int equipIdx = InputHandler.GetNumberInput(0, equippedItems.Count, "Enter equipment number to manage, or 0 to go back:");
            if (equipIdx > 0 && equipIdx <= equippedItems.Count)
            {
                var selectedEquip = equippedItems[equipIdx - 1];
                int? action = Printer.PrintItemMenu(selectedEquip);
                HandleItemAction(selectedEquip, action);
                InputHandler.WaitForKey();
            }
        }

        private void HandleItemAction(Entity itemEntity, int? choice)
        {
            if (choice == null) return;

            var itemData = itemEntity.GetComponent<ItemData>();
            var wearable = itemEntity.GetComponent<Wearable>();
            var player = itemData?.PlayerEntity;
            var playerData = player?.GetComponent<PlayerData>();

            var actions = new List<string> { "Remove" };

            if (itemData?.PlayerEntity == player)
            {
                if (wearable != null)
                {
                    actions.Add(wearable.IsEquipped ? "Unequip" : "Equip");
                    if (itemData?.Type == ItemType.Weapon && wearable.IsEquipped)
                        actions.Add("Use");
                }
                else if (itemData?.Type == ItemType.Consumable)
                {
                    actions.Add("Use");
                }
            }

            if (choice < 1 || choice > actions.Count)
            {
                Printer.PrintMessage("Invalid choice.");
                return;
            }

            string action = actions[(int)choice - 1].ToLower();

            switch (action)
            {
                case "remove":
                    World.Instance.RemoveEntity(itemEntity);
                    Printer.PrintMessage($"{itemData?.Name} removed from world.");
                    break;

                case "equip":
                    if (wearable != null && !wearable.IsEquipped)
                    {
                        wearable.TryEquip();
                        Printer.PrintItemEquipped(itemEntity);
                    }
                    else Printer.PrintMessage("Cannot equip this item.");
                    break;

                case "unequip":
                    if (wearable != null && wearable.IsEquipped)
                    {
                        wearable.TryUnequip();
                        Printer.PrintItemUnequipped(itemEntity);
                    }
                    else Printer.PrintMessage("Cannot unequip this item.");
                    break;

                case "use":
                    HandleItemUse(itemEntity, playerData);
                    break;
            }
        }

        private void HandleItemUse(Entity itemEntity, PlayerData? playerData)
        {
            var itemData = itemEntity.GetComponent<ItemData>();
            if (itemData == null || playerData == null) return;

            var allPlayers = World.Instance.GetEntitiesWith<PlayerData>().ToList();
            Printer.PrintMessage($"Choose target for {itemData.Name}:");
            Printer.PrintTargetList(allPlayers);
            int targetIndex = InputHandler.GetNumberInput(1, allPlayers.Count);

            if (targetIndex >= 1 && targetIndex <= allPlayers.Count)
            {
                var target = allPlayers[targetIndex - 1];
                itemEntity.GetComponent<Usable>()?.TryUse(target);
                Printer.PrintItemUsed(itemEntity, target);
            }
            else Printer.PrintMessage("Invalid target selection.");
        }
    }
}
