using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IGameController
    {
        IGameField FirstPlayerField { get; }
        IGameField SecondPlayerField { get; }
        bool GameFinished { get; }

        bool FirstPlayerTurns { get; }
        ShotResult Shoot(CellPosition target);
    }
}
