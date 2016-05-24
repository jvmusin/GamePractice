using System;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
{
    public class EmptyCell : IGameCell
    {
        public CellPosition Position { get; }
        public bool Damaged { get; set; }

        public EmptyCell(CellPosition position)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            Position = position;
        }

        protected bool Equals(EmptyCell other)
        {
            return Equals(Damaged, other.Damaged);
        }

        public override bool Equals(object obj)
        {
            var other = obj as EmptyCell;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return -1;
        }
    }
}
