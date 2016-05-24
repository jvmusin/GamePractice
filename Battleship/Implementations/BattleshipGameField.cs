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
            if (!IsOnField(target))
                return false;

            var cell = this[target];
            if (cell.Damaged)
                return false;
            
            cell.Damaged = true;

            if (this[target].GetType() == typeof (ShipCell))
            {
                var currentShip = ((ShipCell) this[target]).Ship;
                if (currentShip.Killed)
                {
                    foreach (var shipCellPosition in currentShip.Pieces.Select(x => x.Position))
                        DamageEverythingAround(shipCellPosition);
                    return true;
                }

                var diagonalNeighbours = target
                    .ByAngleNeighbours
                    .Where(IsOnField)
                    .Select(position => this[position]);
                foreach (var neighbour in diagonalNeighbours)
                    neighbour.Damaged = true;
            }

            return true;
        }

        private void DamageEverythingAround(CellPosition cell)
        {
            foreach (var neighbour in cell.AllNeighbours.Where(IsOnField))
                this[neighbour].Damaged = true;
        }

        public bool IsOnField(CellPosition cell)
        {
            return
                cell.Row.IsInRange(0, Size.Height) &&
                cell.Column.IsInRange(0, Size.Width);
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
                .All(position => this[position].Damaged == other[position].Damaged &&
                                 this[position].GetType() == other[position].GetType());
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
