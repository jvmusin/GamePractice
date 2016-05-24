using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
{
    public class GameFieldBuilder
    {
        public GameRules Rules { get; }
        public Size FieldSize => Rules.FieldSize;

        private readonly bool[,] field;
        private readonly Dictionary<ShipType, int> shipsLeft;

        public IReadOnlyDictionary<ShipType, int> ShipsLeft => shipsLeft;

        public GameFieldBuilder(GameRules rules)
        {
            Rules = rules;
            field = new bool[FieldSize.Height, FieldSize.Width];
            shipsLeft = rules.ShipsCount.ToDictionary(x => x.Key, x => x.Value);
        }

        public GameFieldBuilder() : this(GameRules.Default)
        {
        }

        public bool TryAddShipCell(CellPosition target)
        {
            if (!IsOnField(target) || this[target])
                return false;
            
            if (HaveConnectedByAngleShip(target))
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

            if (!connectedShips.Any())
            {
                shipsLeft[(ShipType) 1]++;
                return true;
            }

            foreach (var ship in connectedShips)
                shipsLeft[ship]--;
//            if (shipsLeft.Keys.Any(x => x < 0))
//            {
//                foreach (var ship in connectedShips)
//                    shipsLeft[ship]++;
//                this[target] = true;
//                return false;
//            }

            var oldShip = (ShipType) connectedShips.Sum(x => x.GetLength()) + 1;
            shipsLeft[oldShip]++;
            return true;
        }

        private bool HaveConnectedByAngleShip(CellPosition position)
        {
            return position.ByAngleNeighbours.Where(IsOnField).Any(x => this[x]);
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
            if (ShipsLeft.Values.Any(x => x > 0))
                throw new InvalidOperationException("Field isn't filled yet");

            var newField = new IGameCell[FieldSize.Height, FieldSize.Width];
            foreach (var row in Enumerable.Range(0, FieldSize.Height))
                foreach (var column in Enumerable.Range(0, FieldSize.Width))
                {
                    if (newField[row, column] != null)
                        continue;

                    var curCell = new CellPosition(row, column);
                    if (!this[curCell])
                    {
                        newField[row, column] = new EmptyCell(curCell);
                        continue;
                    }
                    
                    var curShip = new Ship(EnumerateShipCells(curCell));
                    foreach (var piece in curShip.Pieces)
                    {
                        var pos = piece.Position;
                        newField[pos.Row, pos.Column] = piece;
                    }
                }

            return new GameField(Rules,
                position => newField[position.Row, position.Column]);
        }

        private IEnumerable<CellPosition> EnumerateShipCells(CellPosition start)
        {
            var deltaRight = new CellPosition(0, 1);
            var deltaDown = new CellPosition(1, 0);

            var nextToRight = start.AddDelta(deltaRight);
            var needToGoRight = IsOnField(nextToRight) && this[nextToRight];

            var current = start;
            while (IsOnField(current) && this[current])
            {
                yield return current;
                current = current.AddDelta(needToGoRight ? deltaRight : deltaDown);
            }
        }

        private bool IsOnField(CellPosition position)
        {
            return
                position.Row.IsInRange(0, FieldSize.Height) &&
                position.Column.IsInRange(0, FieldSize.Width);
        }

        public bool this[CellPosition position] {
            get { return field[position.Row, position.Column]; }
            private set { field[position.Row, position.Column] = value; }
        }
    }
}
