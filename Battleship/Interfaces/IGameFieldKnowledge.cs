using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IGameFieldKnowledge : IRectangularReadonlyField<bool?>
    {
        new bool? this[CellPosition position] { get; set; }
    }
}
