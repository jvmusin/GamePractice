using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IBattleshipGameField : IRectangleReadonlyField<IGameCell>
    {
        void Put(Ship ship, int row, int column, bool vertical);
        bool Shoot(int row, int column);
        bool IsAvailablePositionFor(ShipType type, int row, int column, bool vertical);
    }
}
