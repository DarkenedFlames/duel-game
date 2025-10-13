using System.Diagnostics.CodeAnalysis;

namespace CBA
{
    public static class Helper
    {
        [return: NotNull]
        public static T ThisIsNotNull<T>(
            [NotNull] T? value,
            string? message = null)
        {
            return value ?? throw new NullReferenceException(message ?? "Unexpected null value.");
        }

        public static void NotAllAreNull(string? message = null, params object?[] values)
        {
            bool allNull = true;
            foreach (var value in values)
            {
                if (value is not null)
                {
                    allNull = false;
                    break;
                }
            }

            if (allNull)
                throw new NullReferenceException(message ?? "All provided values were null, expected at least one non-null value.");
        }
    }
}
