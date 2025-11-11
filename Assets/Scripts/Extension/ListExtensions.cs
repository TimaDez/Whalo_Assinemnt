using System;
using System.Collections.Generic;

namespace Extension
{
    public static class ListExtensions
    {
        /// <summary>
        /// Fisher–Yates shuffle (in-place).
        /// Produces a perfectly unbiased shuffle.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list, Random rng = null)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            rng ??= new Random(unchecked((int)DateTime.UtcNow.Ticks));

            for (int i = list.Count - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1); // range: [0, i]
                (list[i], list[j]) = (list[j], list[i]); // swap
            }
        }
        
        /// <summary>
        /// Returns a new list containing a shuffled copy of the original list.
        /// Original list is NOT modified.
        /// </summary>
        public static List<T> Shuffled<T>(this IList<T> list, Random rng = null)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            rng ??= new Random(unchecked((int)DateTime.UtcNow.Ticks));

            // create a copy
            var copy = new List<T>(list);

            // Fisher–Yates shuffle
            for (int i = copy.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (copy[i], copy[j]) = (copy[j], copy[i]); // swap
            }

            return copy;
        }
    }

}