using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class RandomPlayer : Player
    {
        public RandomPlayer(IGameField selfField)
            : base(selfField, () => CellPosition.Random(selfField.Size))
        {
        }
    }
}
