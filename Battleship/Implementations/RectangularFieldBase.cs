using System.Drawing;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
{
    public class RectangularFieldBase<T> : IRectangularReadonlyField<T>
    {
        protected readonly T[,] Field;
        public Size Size => new Size(Field.GetHeight(), Field.GetWidth());

        public RectangularFieldBase(Size size)
        {
            Field = new T[size.Height, size.Width];
        }

        public T this[CellPosition position]
        {
            get { return Field.GetValue(position); }
            protected set { Field.SetValue(position, value); }
        }
    }
}
