using System.Collections.Generic;
using Battleship.Implementations;
using Battleship.Utilities;

namespace Battleship.Interfaces
{
    public interface IBattleshipGameField : IRectangularReadonlyField<IGameCell>
    {
        bool Shoot(CellPosition target);
        IReadOnlyDictionary<ShipType, int> SurvivedShips { get; }
    }
}
