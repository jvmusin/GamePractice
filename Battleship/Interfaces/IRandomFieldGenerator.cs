using System;
using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IRandomFieldGenerator
    {
        IGameField Generate();
        IGameField Generate(Predicate<CellPosition> canUseCell);
    }
}