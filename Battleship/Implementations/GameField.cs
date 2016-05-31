using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
{
    public class GameField : IGameField
    {
        public GameRules Rules { get; }
        public Size Size => Rules.FieldSize;
        public IReadOnlyDictionary<ShipType, int> SurvivedShips => survivedShips;

        private readonly IGameCell[,] field;
        private readonly Dictionary<ShipType, int> survivedShips;

        internal GameField(GameRules rules, Func<CellPosition, IGameCell> getCell)
        {
            Rules = rules;
            survivedShips = rules.ShipsCount.ToDictionary(x => x.Key, x => x.Value);
            field = new IGameCell[Size.Height, Size.Width];
            FillField(getCell);
        }

        private void FillField(Func<CellPosition, IGameCell> getCell)
        {
            foreach (var position in this.EnumeratePositions())
                this[position] = getCell(position);
        }

        public ShotResult Shoot(CellPosition target)
        {
            if (!IsOnField(target))
                return null;

            var currentCell = this[target];
            if (currentCell.Damaged)
                return null;
            this[target].Damaged = true;

            if (currentCell is IShipCell)
            {
                var currentShip = ((IShipCell) currentCell).Ship;

                if (currentShip.Killed)
                {
                    survivedShips[currentShip.Type]--;
                    var affectedCells = currentShip.Pieces
                        .Select(x => x.Position.AllNeighbours)
                        .SelectMany(Damage);
                    return ShotResult.Kill(target, affectedCells);
                }
                
                return ShotResult.Hit(target, Damage(target.ByVertexNeighbours));
            }

            return ShotResult.Miss(target);
        }

        private IEnumerable<CellPosition> Damage(IEnumerable<CellPosition> targets)
        {
            foreach (var neighbour in targets.Where(pos => IsOnField(pos) && !this[pos].Damaged))
            {
                this[neighbour].Damaged = true;
                yield return neighbour;
            }
        }

        public bool IsOnField(CellPosition cell)
        {
            return
                cell.Row.IsInRange(0, Size.Height) &&
                cell.Column.IsInRange(0, Size.Width);
        }

        public IEnumerable<CellPosition> EnumeratePositions() => field.EnumeratePositions();

        public IGameCell this[CellPosition position]
        {
            get { return field[position.Row, position.Column]; }
            private set { field[position.Row, position.Column] = value; }
        }

        #region ToString, Equals and GetHashCode

        public override string ToString()
        {
            var table = Enumerable.Range(0, Size.Height).Select(row => new char[Size.Width]).ToArray();
            foreach (var position in this.EnumeratePositions())
            {
                var cell = this[position];
                char symbol;
                if (cell is IShipCell)
                    symbol = cell.Damaged ? 'X' : 'O';
                else symbol = cell.Damaged ? '♥' : '.';
                table[position.Row][position.Column] = symbol;
            }
            return string.Join("\n", table.Select((x, i) => new string(x)));
        }

        protected bool Equals(IGameField other)
        {
            return Size == other.Size &&
                   other.EnumeratePositions()
                       .All(position => this[position].Damaged == other[position].Damaged &&
                                        this[position].GetType() == other[position].GetType());
        }

        public override bool Equals(object obj)
        {
            var other = obj as IGameField;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return -1;
        }

        #endregion
    }
}
