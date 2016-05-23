namespace Battleship.Interfaces
{
    public interface IBattleshipGame
    {
        IBattleshipGameField GetPlayerGameField();
        IBattleshipGameField GetEnemyGameField();
        bool IsPlayersTurn { get; }
        bool MakeTurn(ITurn turn);
        bool IsOver { get; }
    }
}