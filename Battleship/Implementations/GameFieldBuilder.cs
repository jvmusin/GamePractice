using System;
using System.Collections.Generic;
using System.Linq;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
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

        public bool TryAddShipCell(CellPosition position)
        {
            if (!IsPositionValid(position))
                return false;

            //TODO Add checking position and changing shipsCounter state

            this[position] = true;
            return true;
        }

        public bool TryRemoveShipCell(CellPosition position)
        {
            if (!IsOnField(position) || !this[position])
                return false;

            //TODO Add checking position and changing shipsCounter state

            this[position] = false;
            return true;
        }

        public IBattleshipGameField Build()
        {
            return new BattleshipGameField(10, 10,
                (row, column) => new GameCell(ships[row, column]
                    ? CellType.Ship
                    : CellType.Empty));
        }

        private bool IsPositionValid(CellPosition position)
        {
            return IsOnField(position) && !ships[position.Row, position.Column] &&
                   position.AllNeighbours.Where(IsOnField).All(pos => !this[pos]);
        }

        private bool IsOnField(CellPosition position)
        {
            var row = position.Row;
            var column = position.Column;
            return
                0 <= row && row < height &&
                0 <= column && column < width;
        }

        private bool this[CellPosition position] {
            get { return ships[position.Row, position.Column]; }
            set { ships[position.Row, position.Column] = value; }
        }
    }
}
