using System;
using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IPlayerFactory
    {
        IPlayer CreatePlayer(IGameField selfField, Func<CellPosition> nextTarget);
        IPlayer CreateConsolePlayer(IGameField selfField);
        IPlayer CreateRandomPlayer(IGameField selfField);
    }
}
