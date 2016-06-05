using System.Drawing;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
{
    public class GameFieldKnowledge : IGameFieldKnowledge
    {
        public Size Size { get; }
        private readonly bool?[,] field;

        public GameFieldKnowledge(Size size)
        {
            Size = size;
            field = new bool?[Size.Height, Size.Width];
        }

        public bool? this[CellPosition position]
        {
            get { return field[position.Row, position.Column]; }
            set { field[position.Row, position.Column] = value; }
        }

        public override string ToString()
            => this.ToString(x => x == null ? '.' : (x.Value ? 'X' : '♥'));
    }
}
