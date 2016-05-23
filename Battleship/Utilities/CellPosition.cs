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

        private IEnumerable<CellPosition> GetNeighbours(int[] deltasByRow, int[] deltasByColumn)
        {
            for (var i = 0; i < deltasByRow.Length; i++)
                yield return new CellPosition(
                    Row + deltasByRow[i], 
                    Column + deltasByColumn[i]);
        }

        public IEnumerable<CellPosition> AllNeighbours => GetNeighbours(-1, 0, 1).Where(x => !x.Equals(this));
        public IEnumerable<CellPosition> ByAngleNeighbours => GetNeighbours(-1, 1);
        public IEnumerable<CellPosition> ByEdgeNeighbours => GetNeighbours(new[] {-1, 0, 1, 0}, new[] {0, 1, 0, -1});

        protected bool Equals(CellPosition other)
        {
            return
                Row == other.Row &&
                Column == other.Column;
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
