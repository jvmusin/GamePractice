namespace Battleship.Interfaces
{
    public interface IGameController
    {
        IGameField GetPlayerGameField();
        IGameField GetEnemyGameField();

        bool PlayerTurns { get; }
        bool MakeTurn(ITurn turn);

        bool IsOver { get; }
    }
}
