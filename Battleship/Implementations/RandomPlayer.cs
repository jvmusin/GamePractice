using System;
using System.Linq;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
{
    public class RandomPlayer : Player
    {
        public RandomPlayer(IGameField selfField) : base(selfField)
        {
        }

        private readonly Random rnd = new Random();

        public override CellPosition NextTarget
            => OpponentFieldKnowledge.EnumeratePositions()
                .Where(pos => !OpponentFieldKnowledge[pos].HasValue)
                .Shuffle()
                .First();
    }
}
