using Battleship.Implementations;
using Battleship.Interfaces;

namespace Battleship.Base
{
    public abstract class PlayerBase : IPlayer
    {
        public IGameField SelfField { get; }
        public IGameFieldKnowledge OpponentFieldKnowledge { get; }
        
        protected PlayerBase(IGameField selfField)
        {
            SelfField = selfField;
            OpponentFieldKnowledge = new GameFieldKnowledge(SelfField.Size);
        }

        public abstract CellPosition NextTarget { get; }
    }
}
