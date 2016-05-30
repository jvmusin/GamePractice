using System;
using System.Linq;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class RandomPlayer : Player
    {
        public RandomPlayer(IGameField selfField) : base(selfField)
        {
        }

        private readonly Random rnd = new Random();

        public override CellPosition NextTarget
        {
            get
            {
                var targets = OpponentFieldKnowledge.EnumeratePositions().Where(pos => !OpponentFieldKnowledge[pos].HasValue).ToList();
                return targets[rnd.Next(targets.Count)];
            }
        }
    }
}
