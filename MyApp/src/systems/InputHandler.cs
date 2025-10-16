namespace CBA
{
    public static class InputHandler
    {
        public static int GetNumberInput(int max, string? prompt = null, bool allowZero = true)
        {
            while (true)
            {
                if (!string.IsNullOrWhiteSpace(prompt))
                    Console.Write(prompt);

                string? input = Console.ReadLine();
                if (int.TryParse(input, out int choice))
                {
                    if ((allowZero && choice >= 0 && choice <= max) ||
                        (!allowZero && choice >= 1 && choice <= max))
                        return choice;
                }

                Console.WriteLine($"Please enter a number between {(allowZero ? 0 : 1)} and {max}.");
            }
        }

        public static T? GetChoice<T>(IList<T> items, Func<T, string> labelSelector, string? header = null)
        {
            if (items.Count == 0)
            {
                Console.WriteLine("(None available)");
                return default;
            }

            if (!string.IsNullOrWhiteSpace(header))
            {
                Console.WriteLine();
                Console.WriteLine(header);
            }

            for (int i = 0; i < items.Count; i++)
                Console.WriteLine($"{i + 1}. {labelSelector(items[i])}");

            int choice = GetNumberInput(items.Count, "Select an option (0 to cancel): ");
            return choice == 0 ? default : items[choice - 1];
        }

        public static void WaitForKey()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
    }

}
