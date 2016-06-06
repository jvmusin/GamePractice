using System.Drawing;
using Battleship.Implementations;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Base
{
    public class RectangularFieldBase<T> : IRectangularReadonlyField<T>
    {
        protected readonly T[,] Field;
        public Size Size { get; }

        public RectangularFieldBase(Size size)
        {
            Size = size;
            Field = new T[size.Height, size.Width];
        }

        public T this[CellPosition position]
        {
            get { return Field.GetValue(position); }
            protected set { Field.SetValue(position, value); }
        }
    }
}
