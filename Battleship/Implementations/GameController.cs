using System;
using System.Linq;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class GameController : IGameController
    {
        public IGameField FirstPlayerField { get; }
        public IGameField SecondPlayerField { get; }
        private IGameField EnemyField => FirstPlayerTurns ? SecondPlayerField : FirstPlayerField;

        public bool GameFinished { get; private set; }

        public bool FirstPlayerTurns { get; private set; }

        public GameController(IGameField firstPlayerField, IGameField secondPlayerField)
        {
            if (firstPlayerField == null)
                throw new ArgumentNullException(nameof(firstPlayerField));
            if (secondPlayerField == null)
                throw new ArgumentNullException(nameof(secondPlayerField));

            FirstPlayerField = firstPlayerField;
            SecondPlayerField = secondPlayerField;

            GameFinished = false;
            FirstPlayerTurns = true;
        }
        
        public ShotResult Shoot(CellPosition target)
        {
            var result = EnemyField.Shoot(target);

            var everythingKilled = EnemyField.SurvivedShips.Values.All(x => x == 0);
            if (everythingKilled)
                GameFinished = true;

            if (!GameFinished && result != null && result.Type != ShotType.Miss)
                FirstPlayerTurns ^= true;
            return result;
        }
    }
}
