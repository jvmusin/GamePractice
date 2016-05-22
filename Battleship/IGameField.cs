namespace Battleship
{
    public interface IGameField<T>
    {
        int Height { get; }
        int Width { get; }

        T GetElementAt(int row, int column);
    }
}
