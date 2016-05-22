namespace SudokuSolver
{
    public interface IGameField<T>
    {
        int Height { get; }
        int Width { get; }

        T GetElementAt(int row, int column);
        IGameField<T> SetElementAt(int row, int column, T value);
    }
}
