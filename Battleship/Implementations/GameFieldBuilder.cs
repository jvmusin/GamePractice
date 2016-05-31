using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
{
    public class GameFieldBuilder : IGameFieldBuilder
    {
        public GameRules Rules { get; }
        public Size Size => Rules.FieldSize;

        private readonly bool[,] field;
        private readonly Dictionary<ShipType, int> shipsLeft;

        public IReadOnlyDictionary<ShipType, int> ShipsLeft => shipsLeft;

        public GameFieldBuilder(GameRules rules)
        {
            Rules = rules;
            field = new bool[Size.Height, Size.Width];
            shipsLeft = rules.ShipsCount.ToDictionary(x => x.Key, x => x.Value);
        }

        public GameFieldBuilder() : this(GameRules.Default)
        {
        }

        public bool TryAddShipCell(CellPosition position)
        {
            if (!this.IsOnField(position) || this[position])
                return false;
            
            if (HasConnectedByVertexShips(position))
                return false;

            var connectedShips = GetConnectedShips(position).ToList();
            var newShipLength = connectedShips.Sum(type => type.GetLength()) + 1;

            if (!Enum.IsDefined(typeof (ShipType), newShipLength))
                return false;

            var newShip = (ShipType) newShipLength;

            foreach (var destroyedShip in connectedShips)
                shipsLeft[destroyedShip]++;
            shipsLeft[newShip]--;

            this[position] = true;
            return true;
        }

        public bool TryRemoveShipCell(CellPosition position)
        {
            if (!this.IsOnField(position) || !this[position])
                return false;

            this[position] = false;
            var connectedShips = GetConnectedShips(position).ToList();

            foreach (var ship in connectedShips)
                shipsLeft[ship]--;

            var oldShip = (ShipType) connectedShips.Sum(x => x.GetLength()) + 1;
            shipsLeft[oldShip]++;
            return true;
        }

        public bool TryAddFullShip(ShipType ship, CellPosition start, bool vertical)
        {
            return TryEditFullShip(ship, start, vertical, true);
        }

        public bool TryRemoveFullShip(ShipType ship, CellPosition start, bool vertical)
        {
            return TryEditFullShip(ship, start, vertical, false);
        }

        private bool TryEditFullShip(ShipType ship, CellPosition start, bool vertical, bool add)
        {
            var changeState = add ? (Func<CellPosition, bool>) TryAddShipCell : TryRemoveShipCell;
            var returnState = add ? (Func<CellPosition, bool>) TryRemoveShipCell : TryAddShipCell;
            var cells = EnumerateUnreadyShipCells(ship, start, vertical).ToList();
            for (var i = 0; i < cells.Count; i++)
                if (!changeState(cells[i]))
                {
                    while (--i >= 0)
                        returnState(cells[i]);
                    return false;
                }
            return true;
        }

        public bool CanBeAddedSafely(ShipType ship, CellPosition start, bool vertical)
        {
            return CanBeAddedSafely(ship, start, vertical, x => true);
        }

        public bool CanBeAddedSafely(ShipType ship, CellPosition start, bool vertical,
            Predicate<CellPosition> canUseCell)
        {
            return EnumerateUnreadyShipCells(ship, start, vertical)
                .All(position =>
                    this.IsOnField(position) && !this[position] && canUseCell(position) &&
                    !position.AllNeighbours.Any(x => this.IsOnField(x) && this[x]));
        }

        private bool HasConnectedByVertexShips(CellPosition position)
        {
            return position.ByVertexNeighbours.Any(x => this.IsOnField(x) && this[x]);
        }

        private IEnumerable<ShipType> GetConnectedShips(CellPosition position)
        {
            return position.ByEdgeNeighbours
                .Where(x => this.IsOnField(x) && this[x])
                .Select(x => this.FindAllConnectedByEdgeCells(x, isShip => isShip).Count())
                .Cast<ShipType>();
        }

        public IGameField Build()
        {
            if (ShipsLeft.Values.Any(x => x != 0))
                return null;

            var newField = new IGameCell[Size.Height, Size.Width];
            foreach (var position in this.EnumeratePositions())
            {
                var row = position.Row;
                var column = position.Column;

                if (newField[row, column] != null)
                    continue;
                
                if (!this[position])
                {
                    newField[row, column] = new EmptyCell(position);
                    continue;
                }

                var curShip = new Ship(EnumerateReadyShipCells(position));
                foreach (var piece in curShip.Pieces)
                {
                    var pos = piece.Position;
                    newField[pos.Row, pos.Column] = piece;
                }
            }

            return new GameField(Rules,
                position => newField[position.Row, position.Column]);
        }

        public void Clear()
        {
            foreach (var position in this.EnumeratePositions())
                TryRemoveShipCell(position);
        }

        private IEnumerable<CellPosition> EnumerateReadyShipCells(CellPosition start)
        {
            return this.FindAllConnectedByEdgeCells(start, isShip => isShip).OrderBy(x => x);
        }

        private static IEnumerable<CellPosition> EnumerateUnreadyShipCells(ShipType ship, CellPosition start, bool vertical)
        {
            var delta = vertical ? CellPosition.DeltaDown : CellPosition.DeltaRight;
            for (var i = 0; i < ship.GetLength(); i++, start += delta)
                yield return start;
        }

        public bool this[CellPosition position] {
            get { return field[position.Row, position.Column]; }
            private set { field[position.Row, position.Column] = value; }
        }
    }
}
