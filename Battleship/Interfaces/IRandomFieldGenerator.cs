using System;
using Battleship.Base;
using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IRandomFieldGenerator
    {
        IGameField Generate(Predicate<CellPosition> canUseCell);
    }

    public static class RandomFieldGeneratorExtensions
    {
        public static IGameField Generate(this IRandomFieldGenerator generator)
            => generator.Generate(x => true);
    }
}
