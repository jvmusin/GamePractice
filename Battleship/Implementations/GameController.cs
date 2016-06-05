using System;
using System.Linq;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class GameController : IGameController
    {
        public GameRules Rules { get; }

        public IPlayer FirstPlayer { get; }
        public IPlayer SecondPlayer { get; }

        public IPlayer CurrentPlayer => FirstPlayerTurns ? FirstPlayer : SecondPlayer;
        public IPlayer OpponentPlayer => FirstPlayerTurns ? SecondPlayer : FirstPlayer;

        public bool GameFinished { get; private set; }

        public bool FirstPlayerTurns { get; private set; }

        public GameController(IPlayer firstPlayer, IPlayer secondPlayer)
        {
            if (firstPlayer == null)
                throw new ArgumentNullException(nameof(firstPlayer));
            if (secondPlayer == null)
                throw new ArgumentNullException(nameof(secondPlayer));

            var firstPlayerRules = firstPlayer.SelfField.Rules;
            var secondPlayerRules = secondPlayer.SelfField.Rules;
            if (!firstPlayerRules.Equals(secondPlayerRules))
                throw new ArgumentException("Rules shouldn't differ");

            Rules = firstPlayerRules;

            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;

            GameFinished = false;
            FirstPlayerTurns = true;
        }

        public ShotResult Shoot(CellPosition target)
        {
            if (GameFinished)
                return null;

            var result = OpponentPlayer.SelfField.Shoot(target);
            if (result == null)
                return null;

            var everythingKilled = OpponentPlayer.SelfField.SurvivedShips.Values.All(x => x == 0);
            if (everythingKilled)
                GameFinished = true;

            foreach (var cell in result.AffectedCells)
                CurrentPlayer.OpponentFieldKnowledge[cell] = false;
            CurrentPlayer.OpponentFieldKnowledge[result.Target] = result.Type != ShotType.Miss;

            if (!GameFinished && result.Type == ShotType.Miss)
                FirstPlayerTurns ^= true;
            return result;
        }
    }
}
