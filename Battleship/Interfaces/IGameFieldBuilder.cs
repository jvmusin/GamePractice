using System.Collections.Generic;
using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IGameFieldBuilder : IRectangularReadonlyField<bool>
    {
        GameRules Rules { get; }
        IReadOnlyDictionary<ShipType, int> ShipsLeft { get; }

        bool TryAddShipCell(CellPosition target);
        bool TryRemoveShipCell(CellPosition target);

        IGameField Build();
        void Clear();
        
        IGameField GenerateRandomField();
    }
}