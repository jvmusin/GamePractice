namespace Battleship
{
    public interface IBattleshipGameField
    {
        int Height { get; }
        int Width { get; }

        IGameCell GetElementAt(int row, int column);

        void Put(Ship ship, int row, int column, bool vertical);
        bool Shoot(int row, int column);
    }
}
