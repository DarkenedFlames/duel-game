namespace MyApp;
using System;
using System.Collections.Generic;
using System.Linq;

public class TurnManager
{
    private int turnIndex = 0;
    public static List<Player> players { get; private set; } = new();

    public TurnManager(List<Player> playersList)
    {
        players = playersList ?? new List<Player>();
    }

    // Convenience accessor (turnIndex is clamped before use)
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

        // Main loop — stop when only one (or zero) players remain
        while (players.Count > 1)
        {
            ClampTurnIndex();
            int startIndex = turnIndex;
            var current = players[startIndex];

            //
            // 1) Start-of-turn effects (they may kill the current player)
            //
            for (int i = current.ActiveEffects.Count - 1; i >= 0; i--)
            {
                current.ActiveEffects[i].Tick();

                // remove any dead players immediately
                RemoveDeadPlayers();

                // if current died during effects, break out
                if (!players.Contains(current))
                    break;
            }

            // If game ended during effects, break
            if (players.Count <= 1) break;

            // If current was removed during effects, next player is the one that now sits
            // at startIndex (see explanation below)
            if (!players.Contains(current))
            {
                // clamp turnIndex and continue to next iteration (do not increment)
                if (startIndex >= players.Count) turnIndex = 0;
                else turnIndex = startIndex;
                continue;
            }

            //
            // 2) Give the player an item and replenish their stamina (skip if they died)
            //
            Console.Clear();
            ItemFactory.GiveRandomItem(current);
            current.Stamina.Change(current.MaximumStamina.Value - current.Stamina.Value);
            Console.WriteLine($"\n{current.Name}'s stamina has been replenished!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);



            //
            // 3) Player's menu loop (keeps running until the player ends turn).
            //    We check for death BEFORE each menu invocation so a 0-HP player cannot act.
            //
            bool endTurn = false;
            while (!endTurn)
            {
                if (current.Health.Value <= 0)
                {
                    Console.WriteLine($"{current.Name} has fallen and cannot act.");
                    RemoveDeadPlayers();
                    break;
                }

                endTurn = Printer.PrintMainMenu(current);

                // After each action, remove dead players (other players may have died)
                RemoveDeadPlayers();

                if (players.Count <= 1) break;
                if (!players.Contains(current)) break; // current could have died during their own actions
            }

            if (players.Count <= 1) break;

            //
            // 4) Compute next turnIndex safely:
            //    - if current still exists: next = (indexOf(current) + 1) % players.Count
            //    - if current was removed: the player that was immediately after current
            //      in the list now sits at `startIndex`, so next = startIndex % players.Count
            //
            int pos = players.IndexOf(current);
            if (pos == -1)
            {
                // current removed — next player now occupies the old startIndex
                turnIndex = (players.Count > 0) ? (startIndex % players.Count) : 0;
            }
            else
            {
                turnIndex = (pos + 1) % players.Count;
            }
        }

        // End condition
        if (players.Count == 1)
        {
            Console.Clear();
            Console.WriteLine($"{players[0].Name} is the last player standing! They win!");
        }
        else
        {
            Console.Clear();
            Console.WriteLine("No players remain. Game over.");
        }
    }

    // Remove all players whose health is <= 0 and print announcement.
    // Keep modifications localized here so turnIndex adjustments are consistent.
    private void RemoveDeadPlayers()
    {
        var dead = players.Where(p => p.Health.Value <= 0).ToList();
        if (dead.Count == 0) return;

        foreach (var d in dead)
        {
            Console.WriteLine($"{d.Name} has fallen!");
            players.Remove(d);
        }

        // Ensure turnIndex is a valid index into players after removals
        ClampTurnIndex();
    }

    private void ClampTurnIndex()
    {
        if (players.Count == 0)
        {
            turnIndex = 0;
            return;
        }

        if (turnIndex >= players.Count)
            turnIndex = 0;
        else if (turnIndex < 0)
            turnIndex = 0; // safety in case negatives ever sneak in
    }
}
