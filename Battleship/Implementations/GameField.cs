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
            field.Fill(getCell);
        }
        
        public ShotResult Shoot(CellPosition target)
        {
            if (!this.Contains(target))
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
            foreach (var neighbour in targets.Where(pos => this.Contains(pos) && !this[pos].Damaged))
            {
                this[neighbour].Damaged = true;
                yield return neighbour;
            }
        }

        public IGameCell this[CellPosition position] => field.GetValue(position);

        #region ToString, Equals and GetHashCode

        public override string ToString()
        {
            return this.ToString(x =>
            {
                if (x is IShipCell)
                    return x.Damaged ? 'X' : 'O';
                return x.Damaged ? '♥' : '.';
            });
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
