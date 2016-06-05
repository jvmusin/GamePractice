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
                var targets = Enumerable.Range(0, 100).Select(i => base.NextTarget);
                return targets.GroupBy(x => x, (position, positions) => Tuple.Create(positions.Count(), position))
                    .Max().Item2;


//                var targetsCounter = new Dictionary<CellPosition, int>();
//                foreach (var target in Enumerable.Range(0, 100).Select(i => base.NextTarget))
//                {
//                    if (!targetsCounter.ContainsKey(target))
//                        targetsCounter[target] = 0;
//                    targetsCounter[target]++;
//                }
//                return targetsCounter.Max(x => Tuple.Create(x.Value, x.Key)).Item2;
//
//                return Enumerable.Range(0, 100).Select(i => base.NextTarget)
//                    .GroupBy(x => x, (position, positions) =>
//                    {
//                        var cellPositions = positions.ToList();
//                        cellPositions.Add(position);
//                        return cellPositions;
//                    })
//                    .OrderBy(x => x.Count)
//                    .First().First();
            }
        }
    }
}
