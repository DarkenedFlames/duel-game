// ==================== TurnManager ====================
using System;
using System.Collections.Generic;
using System.Linq;

namespace CBA
{
    public class TurnManager
    {
        public event Action<Entity>? OnTurnStart;
        public event Action<Entity>? OnTurnEnd;

        public void StartGameLoop()
        {
            bool gameEnded = false;

            while (!gameEnded)
            {
                var alivePlayers = World.Instance
                    .GetById(EntityCategory.Player)
                    .Where(p => p.GetComponent<ResourcesComponent>()?.Get("Health") > 0)
                    .ToList();

                if (alivePlayers.Count <= 1)
                {
                    gameEnded = true;
                    break;
                }

                foreach (var player in alivePlayers.ToList())
                {
                    ResourcesComponent resources = player.GetComponent<ResourcesComponent>();
                    if (resources.Get("Health") <= 0)
                        continue;

                    OnTurnStart?.Invoke(player);

                    var playerData = player.GetComponent<PlayerData>();
                    bool menuSignaledEnd = HandlePlayerMenu(player, playerData);

                    OnTurnEnd?.Invoke(player);

                    if (resources.Get("Health") <= 0)
                        World.Instance.RemoveEntity(player);

                    if (menuSignaledEnd)
                    {
                        gameEnded = true;
                        break;
                    }
                }
            }

            // When loop exits, print game over and winner
            var survivors = World.Instance
                .GetById(EntityCategory.Player)
                .Where(p => p.GetComponent<ResourcesComponent>()?.Get("Health") > 0)
                .ToList();

            if (survivors.Count == 1)
            {
                string winnerName = survivors[0].DisplayName;
                Printer.PrintMessage($"\nGame Over! {winnerName} wins!");
            }
            else
            {
                Printer.PrintMessage("\nGame Over! No one survived.");
            }
        }

        private bool HandlePlayerMenu(Entity player, PlayerData? playerData)
        {
            bool endTurn = false;

            while (!endTurn)
            {
                var stillAlive = World.Instance
                    .GetEntitiesWith<PlayerData>()
                    .Where(p => p.GetComponent<ResourcesComponent>()?.Get("Health") > 0)
                    .ToList();

                // signal game end to loop but don't print
                if (stillAlive.Count <= 1)
                    return true;

                Printer.ClearAndHeader($"Main Menu: {playerData?.Name}'s Turn");
                Printer.PrintMenu(new List<string> { "Stats", "Inventory", "Equipment", "Status", "End Turn" });
                int choice = InputHandler.GetNumberInput(1, 5);

                switch (choice)
                {
                    case 1:
                        Printer.ClearAndHeader($"{playerData?.Name}'s Stats");
                        Printer.PrintStats(player);
                        InputHandler.WaitForKey();
                        break;

                    case 2:
                        HandleInventoryMenu(player, playerData);
                        break;

                    case 3:
                        HandleEquipmentMenu(player, playerData);
                        break;

                    case 4:
                        Printer.ClearAndHeader($"{playerData?.Name}'s Status");
                        Printer.PrintEffects(player);
                        InputHandler.WaitForKey();
                        break;

                    case 5:
                        endTurn = true;
                        break;

                    default:
                        Printer.PrintMessage("Invalid choice. Try again.");
                        InputHandler.WaitForKey();
                        break;
                }
            }

            return false; // normal turn end
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
