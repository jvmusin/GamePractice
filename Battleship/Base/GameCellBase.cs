using System;
using Battleship.Implementations;
using Battleship.Interfaces;

namespace Battleship.Base
{
    public class GameCellBase : IGameCell
    {
        public CellPosition Position { get; }
        public bool Damaged { get; set; }

        public GameCellBase(CellPosition position, bool damaged)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            Position = position;
            Damaged = damaged;
        }

        protected virtual bool Equals(IGameCell other)
        {
            return
                Equals(Position, other.Position) &&
                Equals(Damaged, other.Damaged);
        }

        public override bool Equals(object obj)
        {
            var other = obj as IGameCell;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }
}
