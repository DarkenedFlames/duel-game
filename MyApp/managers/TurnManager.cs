using System;
using System.Collections.Generic;
using System.Linq;

namespace MyApp
{
    public class TurnManager
    {
        private int turnIndex = 0;
        public static List<Player> players { get; private set; } = new();

        public TurnManager(List<Player> playersList)
        {
            players = playersList ?? new List<Player>();
        }

        public Player CurrentPlayer
        {
            get
            {
                if (players.Count == 0) throw new InvalidOperationException("No players available.");
                ClampTurnIndex();
                return players[turnIndex];
            }
        }

        public void StartTurns()
        {
            if (players.Count == 0)
            {
                Console.WriteLine("No players added to TurnManager.");
                return;
            }

            while (players.Count > 1)
            {
                ClampTurnIndex();
                var current = CurrentPlayer;

                Console.Clear();

                // START OF TURN SEQUENCE
                if (ProcessStartOfTurnEffects(current)) continue; // skip to next player if current died

                GivePlayerItems(current);
                ReplenishStamina(current);
                HandlePeer(current);
                HandleLuck(current);

                // PLAYER ACTION LOOP
                ProcessPlayerActions(current);

                // ADVANCE TURN
                AdvanceTurn(current);
            }

            AnnounceEndGame();
        }

        // ----- Helper Methods -----

        private bool ProcessStartOfTurnEffects(Player player)
        {
            for (int i = player.ActiveEffects.Count - 1; i >= 0; i--)
            {
                player.ActiveEffects[i].Tick();
                RemoveDeadPlayers();
                if (!players.Contains(player)) return true; // player died during effects
            }
            return false;
        }

        private void GivePlayerItems(Player player)
        {
            Console.Clear();
            ItemFactory.GiveRandomItem(player);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private void ReplenishStamina(Player player)
        {
            Console.Clear();
            player.Resources.Refill("Stamina");
            Console.WriteLine($"\n{player.Name}'s stamina has been replenished!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private void HandlePeer(Player player)
        {
            Console.Clear();
            if (player.Stats.Get("Peer") <= 0) return;

            double chance = player.Stats.Get("Peer") / (player.Stats.Get("Peer") + 100); // hyperbolic scaling
            if (Random.Shared.NextDouble() < chance)
            {
                Console.WriteLine("\nPeer activated! Enemy inventories revealed:");
                foreach (var enemy in players.Where(p => p != player))
                {
                    Console.WriteLine($"- {enemy.Name}: {string.Join(", ", enemy.Inventory.Select(i => i.Name))}");
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }

        private void HandleLuck(Player player)
        {
            Console.Clear();
            if (player.Stats.Get("Luck") <= 0) return;

            double chance = player.Stats.Get("Luck") / (player.Stats.Get("Luck") + 100); // hyperbolic scaling
            if (Random.Shared.NextDouble() < chance)
            {
                Console.WriteLine("\nLuck activated! You received a bonus item:");
                ItemFactory.GiveRandomItem(player);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
        }

        private void ProcessPlayerActions(Player player)
        {
            bool endTurn = false;
            while (!endTurn)
            {
                if (player.Resources.Get("Health") <= 0)
                {
                    Console.WriteLine($"{player.Name} has fallen and cannot act.");
                    RemoveDeadPlayers();
                    break;
                }

                endTurn = Printer.PrintMainMenu(player);
                RemoveDeadPlayers();

                if (players.Count <= 1) break;
                if (!players.Contains(player)) break;
            }
        }

        private void AdvanceTurn(Player current)
        {
            int pos = players.IndexOf(current);
            if (pos == -1)
            {
                // current was removed, next player is at old index
                turnIndex = players.Count > 0 ? turnIndex % players.Count : 0;
            }
            else
            {
                turnIndex = (pos + 1) % players.Count;
            }
        }

        private void AnnounceEndGame()
        {
            Console.Clear();
            if (players.Count == 1)
                Console.WriteLine($"{players[0].Name} is the last player standing! They win!");
            else
                Console.WriteLine("No players remain. Game over.");
        }

        private void RemoveDeadPlayers()
        {
            var dead = players.Where(p => p.Resources.Get("Health") <= 0).ToList();
            if (dead.Count == 0) return;

            foreach (var d in dead)
            {
                Console.WriteLine($"{d.Name} has fallen!");
                players.Remove(d);
            }

            ClampTurnIndex();
        }

        private void ClampTurnIndex()
        {
            if (players.Count == 0) { turnIndex = 0; return; }
            if (turnIndex >= players.Count || turnIndex < 0) turnIndex = 0;
        }
    }
}