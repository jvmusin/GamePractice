using System.Collections.Immutable;
using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IBattleshipGameField : IRectangleReadonlyField<IGameCell>
    {
        bool Shoot(int row, int column);
        ImmutableDictionary<ShipType, int> GetSurvivedShips();
    }
}
