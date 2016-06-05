using System;
using System.Linq;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class SmartestPlayer : SmartPlayer
    {
        public SmartestPlayer(IGameField selfField) : base(selfField)
        {
        }

        public override CellPosition NextTarget
            => Enumerable.Range(0, 50).Select(i => base.NextTarget)
                .GroupBy(x => x, (pos, list) => Tuple.Create(list.Count(), pos))
                .Max().Item2;
    }
}
