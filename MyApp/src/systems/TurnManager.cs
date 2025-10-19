// ==================== TurnManager ====================
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
                List<Entity> alive = [.. World.Instance.GetAllPlayers()];
                if (alive.Count <= 1)
                    break;

                foreach (Entity player in alive)
                {
                    OnTurnStart?.Invoke(player);
                    bool endGame = HandlePlayerTurn(player);
                    OnTurnEnd?.Invoke(player);
                    if (endGame) return;
                }
            }

            List<Entity> survivors = [..World.Instance.GetAllPlayers()];
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
                    {
                        Printer.ClearAndHeader($"{player.DisplayName}'s Inventory");
                        var (item, action) = Printer.ShowItemMenu(player);
                        if (item != null && action != null)
                            ExecuteItemAction(item, action);                            
                        InputHandler.WaitForKey();
                        break;
                    }
                    case 3:
                    {
                        Printer.ClearAndHeader($"{player.DisplayName}'s Equipment");
                        var (item, action) = Printer.ShowItemMenu(player, equipment: true);
                        if (item != null && action != null)
                            ExecuteItemAction(item, action);
                        InputHandler.WaitForKey();
                        break;
                    }
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

        private void ExecuteItemAction(Entity item, string action)
        {
            switch (action)
            {
                case "remove": World.Instance.RemoveEntity(item); break;
                case "equip": item.GetComponent<Wearable>()?.TryEquip(); break;
                case "unequip": item.GetComponent<Wearable>()?.TryUnequip(); break;
                case "use": HandleItemUse(item); break;
            }
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
