// ==================== InputHandler ====================
using System;

namespace CBA
{
    public static class InputHandler
    {
        public static int GetNumberInput(int min, int max, string? prompt = null)
        {
            while (true)
            {
                if (!string.IsNullOrWhiteSpace(prompt))
                    Console.WriteLine(prompt);

                string? input = Console.ReadLine();
                if (int.TryParse(input, out int choice) && choice >= min && choice <= max)
                    return choice;

                Console.WriteLine($"Please enter a number between {min} and {max}.");
            }
        }

        public static void WaitForKey()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }
}
