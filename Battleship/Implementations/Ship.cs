using System.Collections.Generic;
using System.Linq;
using Battleship.Base;
using Battleship.Implementations.GameCells;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class Ship : IShip
    {
        public ShipType Type { get; }
        public int Length => Type.GetLength();

        public int Health => pieces.Count(cell => !cell.Damaged);
        public bool Killed => Health == 0;

        private readonly List<ShipCell> pieces;
        public IEnumerable<IGameCell> Pieces => pieces;

        public Ship(IEnumerable<CellPosition> positions)
        {
            pieces = positions.Select(pos => new ShipCell(pos, this)).ToList();
            Type = (ShipType) pieces.Count;
        }
    }
}
