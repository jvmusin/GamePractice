namespace SudokuSolver
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
