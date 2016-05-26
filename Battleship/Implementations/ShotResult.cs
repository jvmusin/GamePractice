using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship.Implementations
{
    public class ShotResult
    {
        public CellPosition Target { get; }
        public ShotType Type { get; }
        public IEnumerable<CellPosition> AffectedCells { get; }

        private ShotResult(CellPosition target, ShotType type, IEnumerable<CellPosition> affectedCells)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (affectedCells == null)
                affectedCells = Enumerable.Empty<CellPosition>();

            Target = target;
            Type = type;
            AffectedCells = affectedCells.ToList();
        }

        public static ShotResult Miss(CellPosition target) 
            => new ShotResult(target, ShotType.Miss, null);

        public static ShotResult Hit(CellPosition target, IEnumerable<CellPosition> affectedCells)
            => new ShotResult(target, ShotType.Hit, affectedCells);

        public static ShotResult Kill(CellPosition target, IEnumerable<CellPosition> affectedCells)
            => new ShotResult(target, ShotType.Kill, affectedCells);
    }
}
