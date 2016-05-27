using System.Drawing;
using System.Linq;
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

        public bool IsOnField(CellPosition cell)
        {
            return
                cell.Row.IsInRange(0, Size.Height) &&
                cell.Column.IsInRange(0, Size.Width);
        }

        public override string ToString()
        {
            var rows = Enumerable.Range(0, Size.Height).Select(x => new char[Size.Width]).ToArray();
            foreach (var position in this.EnumerateCellPositions())
            {
                char symbol;
                var state = field[position.Row, position.Column];
                if (state == null) symbol = '.';
                else if (state.Value) symbol = 'X';
                else symbol = '♥';
                rows[position.Row][position.Column] = symbol;
            }
            return string.Join("\n", rows.Select(row => new string(row)));
        }
    }
}
