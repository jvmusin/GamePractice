using System.Collections.Generic;
using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IBattleshipGameField : IRectangleReadonlyField<IGameCell>
    {
        bool Shoot(int row, int column);
        IReadOnlyDictionary<ShipType, int> GetSurvivedShips();
    }
}
