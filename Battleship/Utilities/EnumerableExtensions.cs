using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship.Utilities
{
    public static class EnumerableExtensions
    {
        private static readonly Random Random = new Random();

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
            => source.OrderBy(x => Random.Next());

        public static IOrderedEnumerable<T> ThenShuffle<T>(this IOrderedEnumerable<T> source)
            => source.ThenBy(x => Random.Next());
    }
}
