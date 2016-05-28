using System;
using System.Collections.Generic;
using System.Drawing;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public interface IGameFieldBuilder
    {
        GameRules Rules { get; }
        Size FieldSize { get; }
        IReadOnlyDictionary<ShipType, int> ShipsLeft { get; }

        bool TryAddShipCell(CellPosition target);
        bool TryRemoveShipCell(CellPosition target);

        IGameField Build();
        IGameField GenerateRandomField();

        bool this[CellPosition position] { get; }
    }
}