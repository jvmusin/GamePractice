using System;
using System.Collections.Generic;
using System.Linq;
using Battleship.Implementations;
using Battleship.Utilities;

namespace Battleship.Interfaces
{
    public class GameFieldBuilder
    {
        private readonly int height;
        private readonly int width;
        private readonly bool[,] ships;
        private readonly Dictionary<ShipType, int> shipsCounter;

        public IReadOnlyDictionary<ShipType, int> ShipsCounter => shipsCounter; 

        public GameFieldBuilder() : this(10, 10)
        {
        }

        public GameFieldBuilder(int height, int width)
        {
            this.height = height;
            this.width = width;
            ships = new bool[height,width];
            shipsCounter = new Dictionary<ShipType, int>();
            foreach (var type in (ShipType[]) Enum.GetValues(typeof (ShipType)))
                shipsCounter[type] = type.GetLength();
        }

        public bool TryAddShipCell(int row, int column)
        {
            if (!IsPositionValid(row, column))
                return false;

            GetNeighbours(row, column).Take(1).ToList();

            ships[row, column] = true;
            return true;
        }

        public bool TryRemoveShipCell(int row, int column)
        {
            if (!IsOnField(row, column) || !ships[row, column])
                return false;
            ships[row, column] = false;
            return true;
        }

        public IReadOnlyDictionary<ShipType,int> GetUnusedShips()
        {
            return shipsCounter;
        }

        public IBattleshipGameField Build()
        {
            throw new NotImplementedException();
        }

        //  Up, right, down, left
        private static readonly int[] deltasByRow    = {-1, 0, 1, 0};
        private static readonly int[] deltasByColumn = {0, 1, 0, -1};

        private void CountConnectedShips(ISet<CellPosition> visited, CellPosition currentPosition, ref int count)
        {
            visited.Add(currentPosition);
            count++;

            for (var direction = 0; direction < 4; direction++)
            {
                var deltaRow = deltasByRow[direction];
                var deltaColumn = deltasByColumn[direction];
                var nextPosition = new CellPosition(currentPosition.Row + deltaRow, currentPosition.Column + deltaColumn);
                if (IsOnField(nextPosition.Row, nextPosition.Column) && !visited.Contains(nextPosition))
                    CountConnectedShips(visited, nextPosition, ref count);
            }
        }

        private bool IsPositionValid(int row, int column)
        {
            return IsOnField(row, column) && 
                GetNeighbours(row, column).All(x => !ships[x.Row, x.Column]);
        }

        private IEnumerable<CellPosition> GetNeighbours(CellPosition position)
        {
            return null;
//            foreach (var neighbour in position.AllNeighbours.Where(IsOnField).SelectMany(GetNeighbours))
//                yield return 
//            for (var deltaRow = -1; deltaRow <= 1; deltaRow++)
//                for (var deltaColumn = -1; deltaColumn <= 1; deltaColumn++)
//                {
//                    if (deltaRow == 0 && deltaColumn == 0)
//                        continue;
//
//                    var curRow = row + deltaRow;
//                    var curColumn = column + deltaColumn;
//                    if (IsOnField(curRow, curColumn))
//                        yield return new CellPosition(curRow, curColumn);
//                }
        }

        private bool IsOnField(int row, int column)
        {
            return
                0 <= row && row < height &&
                0 <= column && column < width;
        }

        private bool IsOnField(CellPosition position) => IsOnField(position.Row, position.Column);
    }
}
