﻿using System;
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

        private readonly IGameCell[,] state;
        private readonly Dictionary<ShipType, int> survivedShips;

        #region Constructors

        public GameField(GameRules rules)
        {
            Rules = rules;
            survivedShips = rules.ShipsCount.ToDictionary(x => x.Key, x => x.Value);

            state = new IGameCell[Size.Height, Size.Width];
            foreach (var position in this.EnumerateCellPositions())
                this[position] = new EmptyCell(position);
        }

        public GameField(GameRules rules, Func<CellPosition, IGameCell> getCell) : this(rules)
        {
            foreach (var position in this.EnumerateCellPositions())
                if ((this[position] = getCell(position)) == null)
                    throw new NullReferenceException("Ship can't be null");
        }

        public GameField(IGameField source) : this(source.Rules)
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
                    survivedShips[currentShip.Type]--;
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

        protected bool Equals(IGameField other)
        {
            return Size == other.Size &&
                   other.EnumerateCellPositions()
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