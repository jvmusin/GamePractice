using System.Collections.Generic;
using System.Drawing;
using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IRectangularReadonlyField<out T>
    {
        Size Size { get; }
        T this[CellPosition position] { get; }
        bool IsOnField(CellPosition cell);
        IEnumerable<CellPosition> EnumeratePositions();
    }
}
