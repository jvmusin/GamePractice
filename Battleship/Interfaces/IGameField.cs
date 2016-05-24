using System.Collections.Generic;
using Battleship.Implementations;
using Battleship.Utilities;

namespace Battleship.Interfaces
{
    public interface IGameField : IRectangularReadonlyField<IGameCell>
    {
        GameRules Rules { get; }
        bool Shoot(CellPosition target);
        IReadOnlyDictionary<ShipType, int> SurvivedShips { get; }
    }
}
