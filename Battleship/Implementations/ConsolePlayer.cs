using System;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class ConsolePlayer : Player
    {
        public ConsolePlayer(IGameField selfField) : base(selfField, GetNextTargetFunction(selfField))
        {
        }

        private static Func<CellPosition> GetNextTargetFunction(IGameField selfField)
        {
            return () =>
            {
                var input = Console.ReadLine().Split();
                var row = int.Parse(input[0]);
                var column = int.Parse(input[1]);
                return new CellPosition(row, column);
            };
        }
    }
}
