namespace Battleship
{
    public interface IShip
    {
        ShipType Type { get; }
        int Length { get; }

        int Health { get; }
        bool Killed { get; }

        ShipCell GetPiece(int index);
    }
}