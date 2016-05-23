using System.Collections.Generic;
using System.Linq;

namespace Battleship
{
    public class Ship : IShip
    {
        public ShipType Type { get; }
        public int Length => Type.GetLength();

        private readonly List<ShipCell> pieces;
        public ShipCell GetPiece(int index) => pieces[index];

        public int Health => pieces.Count(cell => !cell.Damaged);
        public bool Killed => Health == 0;

        public Ship(ShipType type)
        {
            Type = type;
            pieces = Enumerable.Range(0, Length).Select(x => new ShipCell()).ToList();
        }
    }
}
