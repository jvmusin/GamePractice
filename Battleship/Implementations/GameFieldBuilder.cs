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
        private readonly Size size;
        private readonly bool[,] field;
        private readonly Dictionary<ShipType, int> shipsLeft;
        private readonly Dictionary<ShipType, int> maxShips;  

        public IReadOnlyDictionary<ShipType, int> ShipsLeft => shipsLeft;

        public GameFieldBuilder(Size size)
        {
            field = new bool[size.Height, size.Width];

            var ships = (ShipType[]) Enum.GetValues(typeof (ShipType));

            maxShips = ships.ToDictionary(x => x, x => 5 - x.GetLength());
            shipsLeft = new Dictionary<ShipType, int>(maxShips);
        }

        public GameFieldBuilder() : this(new Size(10, 10))
        {
        }

        public bool TryAddShipCell(CellPosition target)
        {
            if (!IsOnField(target) || this[target])
                return false;

            var connectedShips = target.ByEdgeNeighbours
                .Where(IsOnField)
                .Select(CountConnectedCells)
                .Cast<ShipType>()
                .ToList();
            var newShipLength = connectedShips.Sum(type => type.GetLength()) + 1;

            if (!Enum.IsDefined(typeof (ShipType), newShipLength))
                return false;

            var newShip = (ShipType) newShipLength;
            if (shipsLeft[newShip] == 0)
                return false;

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
            var connectedShips = target.ByEdgeNeighbours
                .Where(IsOnField)
                .Select(CountConnectedCells)
                .Cast<ShipType>()
                .ToList();

            if (!connectedShips.Any())
            {
                shipsLeft[(ShipType) 1]++;
                return true;
            }

            foreach (var ship in connectedShips)
                shipsLeft[ship]--;
            if (shipsLeft.Keys.Any(x => x < 0))
            {
                foreach (var ship in connectedShips)
                    shipsLeft[ship]++;
                this[target] = true;
                return false;
            }

            var oldShip = (ShipType) connectedShips.Sum(x => x.GetLength()) + 1;
            shipsLeft[oldShip]++;
            return true;
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
                    .Where(x => IsOnField(x) && !visited.Contains(x) && this[start]);
                foreach (var connected in ways)
                {
                    visited.Add(connected);
                    queue.Enqueue(connected);
                }
            }

            return visited.Count;
        }

        public IBattleshipGameField Build()
        {
            if (ShipsLeft.Any(x => x.Value > 0))
                throw new InvalidOperationException("Field can't be built");

            return new BattleshipGameField(new Size(10, 10),
                position => new GameCell(this[position]
                    ? CellType.Ship
                    : CellType.Empty));
        }

        private bool IsOnField(CellPosition position)
        {
            return
                position.Row.IsInRange(0, size.Height) &&
                position.Column.IsInRange(0, size.Width);
        }

        private bool this[CellPosition position] {
            get { return field[position.Row, position.Column]; }
            set { field[position.Row, position.Column] = value; }
        }
    }
}
