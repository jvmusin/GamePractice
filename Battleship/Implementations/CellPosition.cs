using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Battleship.Implementations
{
    public class CellPosition
    {
        private static readonly Random rnd = new Random();
        public static readonly CellPosition DeltaRight = new CellPosition(0, 1);
        public static readonly CellPosition DeltaDown = new CellPosition(1, 0);

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

        public static CellPosition Random(Size size)
        {
            var row = rnd.Next(size.Height);
            var column = rnd.Next(size.Width);
            return new CellPosition(row, column);
        }

        public static CellPosition operator+(CellPosition current, CellPosition other)
        {
            return new CellPosition(current.Row + other.Row, current.Column + other.Column);
        }

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
