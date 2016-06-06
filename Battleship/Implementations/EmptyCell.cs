using Battleship.Base;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class EmptyCell : GameCellBase, IEmptyCell
    {
        public EmptyCell(CellPosition position) : base(position, false)
        {
        }
    }
}
