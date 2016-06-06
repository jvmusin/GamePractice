using System;
using System.Collections.Generic;
using Battleship.Base;
using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IGameFieldBuilder : IRectangularReadonlyField<bool>
    {
        GameRules Rules { get; }
        IReadOnlyDictionary<ShipType, int> ShipsLeft { get; }

        bool TryAddShipCell(CellPosition position);
        bool TryRemoveShipCell(CellPosition position);

        bool CanBeAddedSafely(ShipType ship, CellPosition start, bool vertical);

        bool CanBeAddedSafely(ShipType ship, CellPosition start, bool vertical,
            Predicate<CellPosition> canUseCell);
        bool TryAddFullShip(ShipType ship, CellPosition start, bool vertical);
        bool TryRemoveFullShip(ShipType ship, CellPosition start, bool vertical);
        
        IGameField Build();
        void Clear();
    }
}
