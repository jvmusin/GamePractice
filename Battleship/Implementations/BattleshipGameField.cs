using System;
using System.Collections.Immutable;
using System.Linq;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
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
                .Select(row =>
                    Enumerable.Range(0, width)
                        .Select(i => (IGameCell) new GameCell(CellType.Empty))
                        .ToArray())
                .ToArray();
        }

        public BattleshipGameField(int height, int width, Func<int, int, IGameCell> getCell) : this(height, width)
        {
            foreach (var position in this.EnumerateCellPositions())
            {
                var row = position.Row;
                var column = position.Column;
                var cell = getCell(row, column);
                if (cell == null)
                    throw new NullReferenceException("Ship can't be null");
                state[row][column] = cell;
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
            if (!this.IsOnField(row, column))
                return false;

            var cell = state[row][column];
            if (cell.Damaged)
                return false;

            //  Only when we hitted a ship
            cell.Damaged = true;
            var diagonalNeighbours = this
                .GetDiagonalNeighbours(row, column)
                .Select(position => this.GetElementAt(position));
            foreach (var neighbour in diagonalNeighbours)
                neighbour.Damaged = true;
            return true;
        }

        public bool IsAvailablePositionFor(ShipType type, int row, int column, bool vertical)
        {
            if (!this.IsOnField(row, column) || GetElementAt(row, column).Type == CellType.Ship)
                return false;

            var deltas = GetDeltasToMovePointer(vertical);
            var deltaRow = deltas.Item1;
            var deltaColumn = deltas.Item2;
            for (var i = 0; i < type.GetLength(); i++)
            {
                var curRow = row + deltaRow * i;
                var curColumn = column + deltaColumn * i;
                if (!this.IsOnField(curRow, curColumn) || HasNeighbours(curRow, curColumn))
                    return false;
            }
            return true;
        }

        private bool HasNeighbours(int row, int column)
        {
            return this.Get8Neighbours(row, column)
                .Select(position => this.GetElementAt(position))
                .Any(x => x.Type == CellType.Ship);
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
