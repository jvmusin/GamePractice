namespace Battleship.Utilities
{
    public static class IntExtensions
    {
        public static bool IsInRange(this int number, int from, int to)
        {
            if (from > to)
                return number.IsInRange(to, from);
            return from <= number && number <= to;
        }
    }
}
