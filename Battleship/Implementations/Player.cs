using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public abstract class Player : IPlayer
    {
        public IGameField SelfField { get; }
        public IGameFieldKnowledge OpponentFieldKnowledge { get; }
        
        protected Player(IGameField selfField)
        {
            SelfField = selfField;
            OpponentFieldKnowledge = new GameFieldKnowledge(SelfField.Size);
        }

        public abstract CellPosition NextTarget { get; }
    }
}
