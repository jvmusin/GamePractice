using System;
using System.Linq;

namespace Battleship
{
    public class BattleshipGameField : IBattleshipGameField
    {
        public int Height => state.Length;
        public int Width => state[0].Length;

        private readonly IGameCell[][] state;

        public IGameCell GetElementAt(int row, int column) => state[row][column];

        #region Constructors

        public BattleshipGameField(int height, int width)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException("Size is not correct");

            state = Enumerable
                .Range(0, height)
                .Select(row => new IGameCell[width])
                .ToArray();
        }

        public BattleshipGameField(int height, int width, Func<int, int, IGameCell> getCell) : this(height, width)
        {
            foreach (var position in this.EnumerateCellPositions())
            {
                var row = position.Row;
                var column = position.Column;
                var ship = getCell(row, column);
                state[row][column] = ship;
            }
        }

        public BattleshipGameField(IBattleshipGameField source) : this(source.Height, source.Width)
        {
            foreach (var position in source.EnumerateCellPositions())
            {
                var row = position.Row;
                var column = position.Column;
                state[row][column] = source.GetElementAt(row, column);
            }
        }
        #endregion
        
        public void Put(Ship ship, int row, int column, bool vertical)
        {
            if (!IsAvailablePositionFor(ship.Type, row, column, vertical))
                throw new InvalidOperationException("Unavailable to put a ship here");

            var deltas = GetDeltasToMovePointer(vertical);
            var deltaRow = deltas.Item1;
            var deltaColumn = deltas.Item2;
            for (var i = 0; i < ship.Type.GetLength(); i++)
            {
                var curRow = row + deltaRow * i;
                var curColumn = column + deltaColumn * i;
                state[curRow][curColumn] = ship.GetPiece(i);
            }
        }

        public bool Shoot(int row, int column)
        {
            var cell = state[row][column];
            if (cell.Damaged)
                return false;

            cell.Damaged = true;
            foreach (var neighbour in this.GetDiagonalNeighbours(row, column).Select(pos => this.GetElementAt(pos)))
                neighbour.Damaged = true;
            return true;
        }

        public bool IsAvailablePositionFor(ShipType type, int row, int column, bool vertical)
        {
            if (GetElementAt(row, column) is ShipCell)
                return false;

            var deltas = GetDeltasToMovePointer(vertical);
            var deltaRow = deltas.Item1;
            var deltaColumn = deltas.Item2;
            for (var i = 0; i < type.GetLength(); i++)
            {
                var nextRow = row + deltaRow * i;
                var nextColumn = column + deltaColumn * i;
                if (!this.IsOnField(nextRow, nextColumn) || HasNeighbours(nextRow, nextColumn))
                    return false;
            }
            return true;
        }

        private bool HasNeighbours(int row, int column)
        {
            return this.Get8Neighbours(row, column)
                .Select(position => this.GetElementAt(position))
                .Any(x => x is ShipCell);
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
                .Select(row => string.Join("", this.GetRow(row)));
            return string.Join("\n", rows);
        }

        #region Equals and HashCode

        protected bool Equals(IBattleshipGameField other)
        {
            if (Height != other.Height || Width != other.Width)
                return false;

            return other.EnumerateCellPositions()
                .All(position =>
                {
                    var currentCell = this.GetElementAt(position);
                    var otherCell = other.GetElementAt(position);
                    if (currentCell.GetType() != otherCell.GetType()) return false;
                    return currentCell.Damaged == otherCell.Damaged;
                });
        }

        public override bool Equals(object obj)
        {
            var other = obj as IBattleshipGameField;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
