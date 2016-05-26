using System;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class Turn : ITurn
    {
        public CellPosition Target { get; }

        public Turn(CellPosition target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            Target = target;
        }
    }
}