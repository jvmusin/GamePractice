using System.Collections.Generic;
using Battleship.Implementations;
using Battleship.Utilities;

namespace Battleship.Interfaces
{
    public interface IGameField : IRectangularReadonlyField<IGameCell>
    {
        GameRules Rules { get; }
        ShotResult Shoot(CellPosition target);
        IReadOnlyDictionary<ShipType, int> SurvivedShips { get; }
    }
}
