using Battleship.Utilities;

namespace Battleship.Interfaces
{
    public interface ITurn
    {
        CellPosition ShotPosition { get; }
    }
}
