using System.Collections.Immutable;
using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IGameFieldBuilder
    {
        bool TryAddShipCell(int row, int column);
        bool TryRemoveShipCell(int row, int column);
        ImmutableDictionary<ShipType,int> GetNotUsedShips();
        IBattleshipGame CreateGame();
    }
}