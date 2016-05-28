using System;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class PlayerFactory : IPlayerFactory
    {
        public IPlayer CreatePlayer(IGameField selfField, Func<CellPosition> nextTarget)
            => new Player(selfField, nextTarget);

        public IPlayer CreateConsolePlayer(IGameField selfField)
            => new Player(selfField, () =>
            {
                var line = Console.ReadLine().Split(' ');
                var row = int.Parse(line[0]);
                var column = int.Parse(line[1]);
                return new CellPosition(row, column);
            });

        public IPlayer CreateRandomPlayer(IGameField selfField)
            => new Player(selfField, () => CellPosition.Random(selfField.Size));
    }
}
