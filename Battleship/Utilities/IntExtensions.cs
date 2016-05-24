using System;

namespace Battleship.Utilities
{
    public static class IntExtensions
    {
        public static bool IsInRange(this int number, int fromInclusive, int toExclusive)
        {
            if (toExclusive < fromInclusive)
                throw new InvalidOperationException(nameof(toExclusive) + " lesser than " + nameof(fromInclusive));
            return fromInclusive <= number && number < toExclusive;
        }
    }
}
