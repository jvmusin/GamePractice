using System;
using System.Linq;

namespace Battleship
{
    public class BattleshipGameField : IGameField<Ship>
    {
        public int Height => state.Length;
        public int Width => state[0].Length;

        private readonly Ship[][] state;

        public Ship GetElementAt(int row, int column) => state[row][column];

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
            var deltas = GetDeltasToMovePointer(vertical);
            var deltaRow = deltas.Item1;
            var deltaColumn = deltas.Item2;
            for (var i = 0; i < shipType.GetLength(); i++)
            {
                if (!this.IsOnField(row, column) || HasChildhoods(row, column))
                    return false;
                row += deltaRow;
                column += deltaColumn;
            }
            return true;
        }

        private bool HasChildhoods(int row, int column)
        {
            for (var deltaRow = -1; deltaRow <= 1; deltaRow++)
                for (var deltaColumn = -1; deltaColumn <= 1; deltaColumn++)
                {
                    if (deltaRow == 0 && deltaColumn == 0)
                        continue;
                    var nextRow = row + deltaRow;
                    var nextColumn = column + deltaColumn;
                    if (this.IsOnField(nextRow, nextColumn) && state[nextRow][nextColumn] != null)
                        return true;
                }
            return false;
        }

        public BattleshipGameField Put(Ship ship, int row, int column, bool vertical)
        {
            if (!IsAvailablePositionFor(ship.Type, row, column, vertical))
                throw new InvalidOperationException("Unavailable to put a ship here");

            var newField = new BattleshipGameField(this);
            var deltas = GetDeltasToMovePointer(vertical);
            var deltaRow = deltas.Item1;
            var deltaColumn = deltas.Item2;
            for (var i = 0; i < ship.Type.GetLength(); i++)
            {
                newField.state[row][column] = ship;
                row += deltaRow;
                column += deltaColumn;
            }

            return newField;
        }

        private static Tuple<int, int> GetDeltasToMovePointer(bool vertical)
        {
            return vertical
                ? Tuple.Create(1, 0)
                : Tuple.Create(0, 1);
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
