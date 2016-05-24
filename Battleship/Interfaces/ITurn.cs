using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface ITurn
    {
        CellPosition ShotPosition { get; }
    }
}
