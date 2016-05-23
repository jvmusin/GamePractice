using System.Collections.Generic;
using System.Linq;

namespace Battleship
{
    public interface IRectangleReadonlyField<out T>
    {
        int Height { get; }
        int Width { get; }
        T GetElementAt(int row, int column);
    }

    public static class RectangleReadonlyFieldExtensions
    {
        public static IEnumerable<T> GetRow<T>(this IRectangleReadonlyField<T> field, int row)
        {
            return Enumerable.Range(0, field.Width)
                .Select(column => field.GetElementAt(row, column));
        }

        public static IEnumerable<T> GetColumn<T>(this IRectangleReadonlyField<T> field, int column)
        {
            return Enumerable.Range(0, field.Height)
                .Select(row => field.GetElementAt(row, column));
        }

        public static bool IsOnField<T>(this IRectangleReadonlyField<T> field, int row, int column)
        {
            var height = field.Height;
            var width = field.Width;
            return
                0 <= row && row < height &&
                0 <= column && column < width;
        }

        public static IEnumerable<CellPosition> EnumerateCellPositions<T>(this IRectangleReadonlyField<T> field)
        {
            return
                from row in Enumerable.Range(0, field.Height)
                from column in Enumerable.Range(0, field.Width)
                select new CellPosition(row, column);
        }

        #region Using CellPosition utility class

        public static T GetElementAt<T>(this IRectangleReadonlyField<T> field, CellPosition position)
        {
            return field.GetElementAt(position.Row, position.Column);
        }

        public static bool IsOnField<T>(this IRectangleReadonlyField<T> field, CellPosition position)
        {
            return field.IsOnField(position.Row, position.Column);
        }

        #endregion
    }
}
