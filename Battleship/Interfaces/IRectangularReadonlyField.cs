using System.Drawing;
using Battleship.Utilities;

namespace Battleship.Interfaces
{
    public interface IRectangularReadonlyField<out T>
    {
        Size Size { get; }
        T this[CellPosition position] { get; }
        bool Contains(CellPosition position);
    }
}
