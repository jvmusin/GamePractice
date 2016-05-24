using System.Drawing;
using Battleship.Utilities;

namespace Battleship.Interfaces
{
    public interface IRectangularReadonlyField<out T>
    {
        Size Size { get; }
        T this[CellPosition position] { get; }
        bool IsOnField(CellPosition cell);
    }
}
