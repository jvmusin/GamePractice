namespace Battleship.Utilities
{
    public static class IntExtensions
    {
        public static bool IsInRange(this int number, int fromInclusive, int toExclusive)
            => fromInclusive <= number && number < toExclusive;
    }
}
