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
        {
            get
            {
                return Enumerable.Range(0, 100).Select(i => base.NextTarget)
                    .GroupBy(x => x, (position, positions) =>
                    {
                        var cellPositions = positions.ToList();
                        cellPositions.Add(position);
                        return cellPositions;
                    })
                    .OrderBy(x => x.Count)
                    .First().First();
            }
        }
    }
}
