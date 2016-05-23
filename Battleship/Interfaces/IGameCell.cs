using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IGameCell
    {
        CellType Type { get; }
        bool Damaged { get; set; }
    }
}
