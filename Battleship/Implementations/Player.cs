using System;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class Player : IPlayer
    {
        public IGameField SelfField { get; }
        public IGameFieldKnowledge OpponentFieldKnowledge { get; }

        public Player(IGameField selfField, Func<CellPosition> nextTarget)
        {
            SelfField = selfField;
            this.nextTarget = nextTarget;
            OpponentFieldKnowledge = new GameFieldKnowledge(SelfField.Size);
        }

        protected Player(IGameField selfField)
        {
            SelfField = selfField;
            OpponentFieldKnowledge = new GameFieldKnowledge(SelfField.Size);
        }

        private readonly Func<CellPosition> nextTarget;
        public virtual CellPosition NextTarget => nextTarget();
    }
}
