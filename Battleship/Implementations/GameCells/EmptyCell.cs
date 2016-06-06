using Battleship.Base;
using Battleship.Interfaces;

namespace Battleship.Implementations.GameCells
{
    public class EmptyCell : GameCellBase, IEmptyCell
    {
        public EmptyCell(CellPosition position) : base(position, false)
        {
        }
    }
}
