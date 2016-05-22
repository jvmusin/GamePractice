using System.Collections.Generic;
using System.Linq;

namespace Battleship
{
    public static class GameFieldExtensions
    {
        public static IEnumerable<CellPosition> EnumerateCellPositions<T>(this IGameField<T> field)
        {
            return
                from row in Enumerable.Range(0, field.Height)
                from column in Enumerable.Range(0, field.Width)
                select new CellPosition(row, column);
        }
        
        public static T GetElementAt<T>(this IGameField<T> field, CellPosition position)
        {
            return field.GetElementAt(position.Row, position.Column);
        }

        public static IEnumerable<T> GetRow<T>(this IGameField<T> field, int row)
        {
            return Enumerable.Range(0, field.Width)
                .Select(column => field.GetElementAt(row, column));
        }

        public static IEnumerable<T> GetColumn<T>(this IGameField<T> field, int column)
        {
            return Enumerable.Range(0, field.Height)
                .Select(row => field.GetElementAt(row, column));
        }

        public static bool IsOnField<T>(this IGameField<T> field, int row, int column)
        {
            var height = field.Height;
            var width = field.Width;
            return
                0 <= row && row < height &&
                0 <= column && column < width;
        }

        public static bool IsOnField<T>(this IGameField<T> field, CellPosition position)
        {
            return field.IsOnField(position.Row, position.Column);
        }
    }
}
