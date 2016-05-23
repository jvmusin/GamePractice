namespace Battleship
{
    public enum ShipType
    {
        Submarine       = 1,
        Destroyer       = 2,
        Cruiser         = 3,
        Battleship      = 4,
    }

    public static class ShipTypeExtensions
    {
        public static int GetLength(this ShipType type) => (int) type;
    }
}
