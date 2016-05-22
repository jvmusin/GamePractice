using System;
using System.Linq;
using SudokuSolver;

namespace Battleship
{
    public class BattleshipGameField : IGameField<Ship>
    {
        public int Height => state.Length;
        public int Width => state[0].Length;

        private readonly Ship[][] state;

        public Ship GetElementAt(int row, int column) => state[row][column];

        public IGameField<Ship> SetElementAt(int row, int column, Ship value)
        {
            var result = new BattleshipGameField(this);
            result.state[row][column] = value;
            return result;
        }

        public BattleshipGameField(int height, int width)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Size is not correct");

            state = Enumerable
                .Range(0, height)
                .Select(row => new Ship[width])
                .ToArray();
        }

        public BattleshipGameField(int height, int width, Func<int, int, Ship> getShip) : this(height, width)
        {
            foreach (var position in this.EnumerateCellPositions())
            {
                var row = position.Row;
                var column = position.Column;
                var ship = getShip(row, column);
                state[row][column] = ship;
            }
        }

        public BattleshipGameField(IGameField<Ship> source) : this(source.Height, source.Width)
        {
            foreach (var position in source.EnumerateCellPositions())
            {
                var row = position.Row;
                var column = position.Column;
                state[row][column] = source.GetElementAt(row, column);
            }
        }

        public bool IsAvailablePositionFor(ShipType shipType, int row, int column, bool vertical)
        {
            var deltaRow = vertical ? 1 : 0;
            var deltaColumn = vertical ? 0 : 1;
            for (var i = 0; i < shipType.GetLength(); i++)
            {
                row += deltaRow;
                column += deltaColumn;
                if (!this.IsOnField(row, column) || state[row][column] != null)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            var rows = Enumerable.Range(0, Height)
                .Select(row => string.Join("", this.GetRow(row).Select(ship => ship.Id)));
            return string.Join("\n", rows);
        }

        #region Equals and HashCode

        protected bool Equals(IGameField<Ship> other)
        {
            if (Height != other.Height || Width != other.Width)
                return false;

            var haveDifferentCells = other.EnumerateCellPositions()
                .Any(pos => !this.GetElementAt(pos).Equals(other.GetElementAt(pos)));

            return !haveDifferentCells;
        }

        public override bool Equals(object obj)
        {
            var other = obj as IGameField<int>;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
