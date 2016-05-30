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

        public bool TryAddShipCell(CellPosition target)
        {
            if (!IsOnField(target) || this[target])
                return false;
            
            if (HasConnectedByAngleShips(target))
                return false;

            var connectedShips = GetConnectedShips(target).ToList();
            var newShipLength = connectedShips.Sum(type => type.GetLength()) + 1;

            if (!Enum.IsDefined(typeof (ShipType), newShipLength))
                return false;

            var newShip = (ShipType) newShipLength;
//            if (shipsLeft[newShip] == 0)
//                return false;

            foreach (var destroyedShip in connectedShips)
                shipsLeft[destroyedShip]++;
            shipsLeft[newShip]--;

            this[target] = true;
            return true;
        }

        public bool TryRemoveShipCell(CellPosition target)
        {
            if (!IsOnField(target) || !this[target])
                return false;

            this[target] = false;
            var connectedShips = GetConnectedShips(target).ToList();

            foreach (var ship in connectedShips)
                shipsLeft[ship]--;

            var oldShip = (ShipType) connectedShips.Sum(x => x.GetLength()) + 1;
            shipsLeft[oldShip]++;
            return true;
        }

        private bool HasConnectedByAngleShips(CellPosition position)
        {
            return position.ByAngleNeighbours.Any(x => IsOnField(x) && this[x]);
        }

        private IEnumerable<ShipType> GetConnectedShips(CellPosition position)
        {
            return position.ByEdgeNeighbours
                .Where(x => IsOnField(x) && this[x])
                .Select(CountConnectedCells)
                .Cast<ShipType>();
        }

        private int CountConnectedCells(CellPosition start)
        {
            var visited = new HashSet<CellPosition> {start};
            var queue = new Queue<CellPosition>();
            queue.Enqueue(start);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                var ways = current.ByEdgeNeighbours
                    .Where(x => IsOnField(x) && !visited.Contains(x) && this[x]);
                foreach (var connected in ways)
                {
                    visited.Add(connected);
                    queue.Enqueue(connected);
                }
            }

            return visited.Count;
        }

        public IGameField Build()
        {
            if (ShipsLeft.Values.Any(x => x != 0))
                return null;

            var newField = new IGameCell[Size.Height, Size.Width];
            foreach (var position in this.EnumerateCellPositions())
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

                var curShip = new Ship(EnumerateShipCells(position));
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
            foreach (var position in this.EnumerateCellPositions())
                TryRemoveShipCell(position);
        }

        private IEnumerable<CellPosition> EnumerateShipCells(CellPosition start)
        {
            var nextToRight = start + CellPosition.DeltaRight;
            var needToGoRight = IsOnField(nextToRight) && this[nextToRight];
            
            while (IsOnField(start) && this[start])
            {
                yield return start;
                start += needToGoRight ? CellPosition.DeltaRight  : CellPosition.DeltaDown;
            }
        }

        public bool IsOnField(CellPosition position)
        {
            return
                position.Row.IsInRange(0, Size.Height) &&
                position.Column.IsInRange(0, Size.Width);
        }

        public IGameField GenerateRandomField() => new RandomFieldGenerator(this).Generate();

        public bool this[CellPosition position] {
            get { return field[position.Row, position.Column]; }
            private set { field[position.Row, position.Column] = value; }
        }
    }
}
