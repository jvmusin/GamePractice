using System;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
{
    public class ShipCell : IGameCell
    {
        public CellPosition Position { get; }
        public bool Damaged { get; set; }
        public IShip Parent { get; }

        public ShipCell(CellPosition position, IShip parent)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            Position = position;
            Parent = parent;
        }

        protected bool Equals(ShipCell other)
        {
            return
                Equals(Position, other.Position) &&
                Equals(Damaged, other.Damaged) &&
                Equals(Parent, other.Parent);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ShipCell;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return -1;
        }
    }
}
