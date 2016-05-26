using System.Collections.Generic;
using Battleship.Implementations;

namespace Battleship.Interfaces
{
    public interface IShip
    {
        ShipType Type { get; }
        int Length { get; }

        int Health { get; }
        bool Killed { get; }

        IEnumerable<IGameCell> Pieces { get; } 
    }
}
