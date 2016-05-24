using System.Collections.Generic;
using System.Linq;

namespace Battleship.Utilities
{
    public class CellPosition
    {
        public int Row { get; }
        public int Column { get; }

        public CellPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        private IEnumerable<CellPosition> GetNeighbours(params int[] deltas)
        {
            return
                from deltaRow in deltas
                from deltaColumn in deltas
                select new CellPosition(Row + deltaRow, Column + deltaColumn);
        }

        public IEnumerable<CellPosition> AllNeighbours => GetNeighbours(-1, 0, 1).Where(x => !x.Equals(this));
        public IEnumerable<CellPosition> ByAngleNeighbours => GetNeighbours(-1, 1);
        public IEnumerable<CellPosition> ByEdgeNeighbours => AllNeighbours.Except(ByAngleNeighbours);

        public CellPosition AddDelta(CellPosition delta) => new CellPosition(Row + delta.Row, Column + delta.Column);

        protected bool Equals(CellPosition other)
        {
            return
                Equals(Row, other.Row) &&
                Equals(Column, other.Column);
        }

        public override bool Equals(object obj)
        {
            var other = obj as CellPosition;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Row ^ Column;
        }

        public override string ToString()
        {
            return $"Row: {Row}, Column: {Column}";
        }
    }
}
