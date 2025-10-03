using System;
using System.Collections.Generic;

namespace MyApp
{
    public static class WeightedRandom
    {
        private static readonly Random rng = new Random();

        public static T Choose<T>(Dictionary<T, double> weights) where T : notnull
        {
            double total = 0;
            foreach (var w in weights.Values)
                total += w;

            double roll = rng.NextDouble() * total;
            foreach (var kvp in weights)
            {
                if (roll < kvp.Value)
                    return kvp.Key;
                roll -= kvp.Value;
            }
            throw new Exception("Weighted random selection failed");
        }
    }
}
