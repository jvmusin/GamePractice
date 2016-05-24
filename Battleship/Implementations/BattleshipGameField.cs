using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
{
    public class BattleshipGameField : IBattleshipGameField
    {
        public Size Size { get; }
        public IReadOnlyDictionary<ShipType, int> SurvivedShips => survivedShips;

        private readonly IGameCell[,] state;
        private readonly Dictionary<ShipType, int> survivedShips;

        #region Constructors

        public BattleshipGameField(Size size)
        {
            if (!size.Height.IsInRange(1, int.MaxValue) ||
                !size.Width.IsInRange(1, int.MaxValue))
                throw new ArgumentOutOfRangeException(nameof(size));

            Size = size;
            var ships = (ShipType[]) Enum.GetValues(typeof (ShipType));
            survivedShips = ships.ToDictionary(x => x, x => x.GetLength());

            state = new IGameCell[size.Height, size.Width];
            foreach (var position in this.EnumerateCellPositions())
                this[position] = new EmptyCell(position);
        }

        public BattleshipGameField(Size size, Func<CellPosition, IGameCell> getCell) : this(size)
        {
            if (getCell == null)
                throw new ArgumentNullException(nameof(getCell));

            foreach (var position in this.EnumerateCellPositions())
            {
                var cell = getCell(position);
                if (cell == null)
                    throw new NullReferenceException("Ship can't be null");
                this[position] = cell;
            }
        }

        public BattleshipGameField(IBattleshipGameField source) : this(source.Size)
        {
            foreach (var position in source.EnumerateCellPositions())
                this[position] = source[position];
        }

        #endregion

        public bool Shoot(CellPosition target)
        {
            if (!this.IsOnField(target))
                return false;

            var cell = this[target];
            if (cell.Damaged)
                return false;
            
            cell.Damaged = true;

            //TODO Mark all cells around if we killed something
            if (this[target].GetType() == typeof(ShipCell))
            {
                var diagonalNeighbours = target
                    .ByAngleNeighbours
                    .Select(position => this[position]);
                foreach (var neighbour in diagonalNeighbours)
                    neighbour.Damaged = true;


                var currentCell = (ShipCell) this[target];

            }
            return true;
        }

        public bool Contains(CellPosition position)
        {
            return
                position.Row.IsInRange(0, Size.Height) &&
                position.Column.IsInRange(0, Size.Width);
        }

        public IGameCell this[CellPosition position]
        {
            get { return state[position.Row, position.Column]; }
            private set { state[position.Row, position.Column] = value; }
        }

        #region ToString, Equals and GetHashCode

        public override string ToString()
        {
            var rows = Enumerable.Range(0, Size.Height)
                .Select(row => string.Join("", this.GetRow(row)));
            return string.Join("\n", rows);
        }

        protected bool Equals(IBattleshipGameField other)
        {
            if (Size != other.Size)
                return false;

            return other.EnumerateCellPositions()
                .All(position => this[position].Equals(other[position]));
        }

        public override bool Equals(object obj)
        {
            var other = obj as IBattleshipGameField;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return -1;
        }

        #endregion
    }
}
