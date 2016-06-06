using System.Drawing;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
{
    public class GameFieldKnowledge : RectangularFieldBase<bool?>, IGameFieldKnowledge
    {
        public GameFieldKnowledge(Size size) : base(size)
        {
        }

        public override string ToString()
            => this.ToString(x => x == null ? '.' : (x.Value ? 'X' : '♥'));

        public new bool? this[CellPosition position]
        {
            get { return base[position]; }
            set { base[position] = value; }
        }
    }
}
