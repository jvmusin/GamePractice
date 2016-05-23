namespace Battleship.Interfaces
{
    public interface IRectangleReadonlyField<out T>
    {
        int Height { get; }
        int Width { get; }
        T GetElementAt(int row, int column);
    }
}
