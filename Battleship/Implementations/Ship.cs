using System.Collections.Generic;
using System.Linq;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class Ship : IShip
    {
        public ShipType Type { get; }
        public int Length => Type.GetLength();

        public int Health => pieces.Count(cell => !cell.Damaged);
        public bool Killed => Health == 0;

        private readonly List<GameCell> pieces;
        public IGameCell GetPiece(int index) => pieces[index];

        public Ship(ShipType type)
        {
            Type = type;
            pieces = Enumerable.Range(0, Length).Select(x => new GameCell(CellType.Ship)).ToList();
        }
    }
}
