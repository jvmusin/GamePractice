using System.Linq;
using Battleship.Base;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations.Players
{
    public class RandomPlayer : PlayerBase
    {
        public RandomPlayer(IGameField selfField) : base(selfField)
        {
        }

        public override CellPosition NextTarget
            => OpponentFieldKnowledge.EnumeratePositions()
                .Where(pos => !OpponentFieldKnowledge[pos].HasValue)
                .Shuffle()
                .First();
    }
}
