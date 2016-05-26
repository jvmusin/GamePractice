using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class Player : IPlayer
    {
        public IGameField SelfField { get; }
        public IGameFieldKnowledge OpponentFieldKnowledge { get; }

        public Player(IGameField selfField)
        {
            SelfField = selfField;
            OpponentFieldKnowledge = new GameFieldKnowledge(SelfField.Size);
        }
    }
}