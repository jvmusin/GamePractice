using System;
using Battleship.Base;
using Battleship.Interfaces;

namespace Battleship.Implementations.GameCells
{
    public class ShipCell : GameCellBase, IShipCell
    {
        public IShip Ship { get; }

        public ShipCell(CellPosition position, IShip ship) : base(position, false)
        {
            if ((Ship = ship) == null)
                throw new ArgumentNullException(nameof(ship));
        }
    }
}
