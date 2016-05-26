namespace Battleship.Interfaces
{
    public interface IPlayer
    {
        IGameField SelfField { get; }
        IGameFieldKnowledge OpponentFieldKnowledge { get; }
    }
}
