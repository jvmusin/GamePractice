using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Battleship.Implementations
{
    public class CellPosition : IComparable<CellPosition>, IComparable
    {
        private static readonly Random rnd = new Random();

        public static readonly CellPosition DeltaRight = new CellPosition(0, 1);
        public static readonly CellPosition DeltaDown = new CellPosition(1, 0);

        public CellPosition Reversed => new CellPosition(-Row, -Column);

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
        public IEnumerable<CellPosition> ByVertexNeighbours => GetNeighbours(-1, 1);
        public IEnumerable<CellPosition> ByEdgeNeighbours => AllNeighbours.Except(ByVertexNeighbours);

        public static CellPosition Random(Size size)
        {
            var row = rnd.Next(size.Height);
            var column = rnd.Next(size.Width);
            return new CellPosition(row, column);
        }

        public static CellPosition operator +(CellPosition current, CellPosition other)
        {
            return new CellPosition(current.Row + other.Row, current.Column + other.Column);
        }

        public static CellPosition operator *(CellPosition cell, int delta)
        {
            return new CellPosition(cell.Row * delta, cell.Column * delta);
        }

        public int CompareTo(CellPosition other)
        {
            var cmp = Row.CompareTo(other.Row);
            if (cmp == 0) cmp = Column.CompareTo(other.Column);
            return cmp;
        }

        public int CompareTo(object obj)
        {
            var other = obj as CellPosition;
            return other == null ? 0 : CompareTo(other);
        }

        protected bool Equals(CellPosition other)
        {
            return CompareTo(other) == 0;
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
