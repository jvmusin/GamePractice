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
        private readonly bool[,] filled;
        private readonly Dictionary<ShipType, int> shipsCounter;

        public IReadOnlyDictionary<ShipType, int> ShipsCounter => shipsCounter;

        public GameFieldBuilder() : this(new Size(10, 10))
        {
        }

        public GameFieldBuilder(Size size)
        {
            filled = new bool[size.Height, size.Width];
            var ships = (ShipType[]) Enum.GetValues(typeof (ShipType));
            shipsCounter = ships.ToDictionary(x => x, x => x.GetLength());
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
            return new BattleshipGameField(new Size(10, 10),
                position => new GameCell(this[position]
                    ? CellType.Ship
                    : CellType.Empty));
        }

        private bool IsPositionValid(CellPosition position)
        {
            return IsOnField(position) && !this[position] &&
                   position.AllNeighbours.Where(IsOnField).All(pos => !this[pos]);
        }

        private bool IsOnField(CellPosition position)
        {
            return
                position.Row.IsInRange(0, size.Height) &&
                position.Column.IsInRange(0, size.Width);
        }

        private bool this[CellPosition position] {
            get { return filled[position.Row, position.Column]; }
            set { filled[position.Row, position.Column] = value; }
        }
    }
}
