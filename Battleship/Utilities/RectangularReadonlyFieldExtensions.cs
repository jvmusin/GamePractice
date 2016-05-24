using System.Collections.Generic;
using System.Linq;
using Battleship.Interfaces;

namespace Battleship.Utilities
{
    public static class RectangularReadonlyFieldExtensions
    {
        public static IEnumerable<T> GetRow<T>(this IRectangularReadonlyField<T> field, int row)
        {
            return Enumerable.Range(0, field.Size.Width)
                .Select(column => field[new CellPosition(row, column)]);
        }

        public static IEnumerable<T> GetColumn<T>(this IRectangularReadonlyField<T> field, int column)
        {
            return Enumerable.Range(0, field.Size.Height)
                .Select(row => field[new CellPosition(row, column)]);
        }

        public static IEnumerable<CellPosition> EnumerateCellPositions<T>(this IRectangularReadonlyField<T> field)
        {
            return
                from row in Enumerable.Range(0, field.Size.Height)
                from column in Enumerable.Range(0, field.Size.Width)
                select new CellPosition(row, column);
        }
    }
}
