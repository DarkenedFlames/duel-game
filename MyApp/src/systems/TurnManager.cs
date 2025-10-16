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
            while (true)
            {
                var alive = World.Instance.GetAllPlayers().Where(p => p.GetComponent<ResourcesComponent>().Get("Health") > 0).ToList();
                if (alive.Count <= 1)
                    break;

                foreach (var player in alive)
                {
                    OnTurnStart?.Invoke(player);
                    bool endGame = HandlePlayerTurn(player);
                    OnTurnEnd?.Invoke(player);
                    if (endGame) return;
                }
            }

            var survivors = World.Instance.GetAllPlayers().Where(p => p.GetComponent<ResourcesComponent>().Get("Health") > 0).ToList();
            if (survivors.Count == 1)
                Printer.PrintMessage($"\nGame Over! {survivors[0].DisplayName} wins!");
            else
                Printer.PrintMessage("\nGame Over! No one survived.");
        }

        private bool HandlePlayerTurn(Entity player)
        {
            while (true)
            {
                int choice = Printer.MultiChoiceList($"{player.DisplayName}'s Turn", 
                    ["Stats", "Inventory", "Equipment", "Status", "End Turn"]);

                switch (choice)
                {
                    case 1:
                        Printer.ClearAndHeader($"{player.DisplayName}'s Stats");
                        Printer.PrintStats(player);
                        InputHandler.WaitForKey();
                        break;
                    case 2:
                        HandleInventoryMenu(player);
                        break;
                    case 3:
                        HandleEquipmentMenu(player);
                        break;
                    case 4:
                        Printer.ClearAndHeader($"{player.DisplayName}'s Status");
                        Printer.PrintEffects(player);
                        InputHandler.WaitForKey();
                        break;
                    case 5:
                        return false; // end turn
                }
            }
        }

        private void HandleInventoryMenu(Entity player)
        {
            var items = World.Instance.GetAllForPlayer<Entity>(player, EntityCategory.Item).ToList();
            var item = InputHandler.GetChoice(items, i => i.DisplayName, $"{player.DisplayName}'s Inventory");
            if (item == null) return;

            HandleItemAction(item);
        }

        private void HandleEquipmentMenu(Entity player)
        {
            var equipped = Printer.PrintEquipment(player);
            var selected = InputHandler.GetChoice(equipped, e => e.DisplayName, $"{player.DisplayName}'s Equipment");
            if (selected == null) return;

            HandleItemAction(selected);
        }

        private void HandleItemAction(Entity item)
        {
            var actions = BuildItemActions(item);

            int choice = Printer.MultiChoiceList
            (
                $"Item Menu: {World.Instance.GetPlayerOf(item).DisplayName} using {item.DisplayName}",
                actions
            );
            if (choice == 0) return;

            string action = actions[choice - 1].ToLower();
            ExecuteItemAction(item, action);
        }

        private List<string> BuildItemActions(Entity item)
        {
            var actions = new List<string> { "Remove" };

            if (item.HasComponent<Usable>() && !item.HasComponent<Wearable>())
                actions.Add("Use");

            if (item.HasComponent<Wearable>())
            {
                var wearable = item.GetComponent<Wearable>();
                if (wearable.IsEquipped)
                {
                    actions.Add("Unequip");
                    if (item.HasComponent<Usable>())
                        actions.Add("Use");
                }
                else actions.Add("Equip");
            }

            return actions;
        }

        private void ExecuteItemAction(Entity item, string action)
        {
            switch (action)
            {
                case "remove":
                    World.Instance.RemoveEntity(item);
                    Printer.PrintMessage($"{item.DisplayName} removed from world.");
                    break;
                case "equip":
                    item.GetComponent<Wearable>()?.TryEquip();
                    break;
                case "unequip":
                    item.GetComponent<Wearable>()?.TryUnequip();
                    break;
                case "use":
                    HandleItemUse(item);
                    break;
            }

            InputHandler.WaitForKey();
        }

        private void HandleItemUse(Entity item)
        {
            var players = World.Instance.GetAllPlayers().ToList();
            var target = InputHandler.GetChoice(players, p => p.DisplayName, $"Choose target for {item.DisplayName}:");
            if (target == null)
            {
                Printer.PrintMessage("Cancelled.");
                return;
            }

            item.GetComponent<Usable>().TryUse(target);
        }
    }

}
