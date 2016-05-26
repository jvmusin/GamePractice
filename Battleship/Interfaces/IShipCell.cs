namespace Battleship.Interfaces
{
    public interface IShipCell : IGameCell
    {
        IShip Ship { get; }
    }
}
