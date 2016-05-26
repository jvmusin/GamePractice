using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IGameController
    {
        IPlayer FirstPlayer { get; }
        IPlayer SecondPlayer { get; }

        IPlayer CurrentPlayer { get; }
        IPlayer OpponentPlayer { get; }

        bool GameFinished { get; }

        bool FirstPlayerTurns { get; }

        ShotResult Shoot(CellPosition target);
    }
}
