using System;
using System.Collections.Generic;
using System.Linq;
using Battleship.Base;

namespace Battleship.Utilities
{
    public static class ArrayExtensions
    {
        public static int GetHeight<T>(this T[,] field)
        {
            return field.GetLength(0);
        }

        public static int GetWidth<T>(this T[,] field)
        {
            return field.GetLength(1);
        }

        public static T GetValue<T>(this T[,] field, CellPosition position)
        {
            return field[position.Row, position.Column];
        }

        public static T SetValue<T>(this T[,] field, CellPosition position, T value)
        {
            return field[position.Row, position.Column] = value;
        }

        public static IEnumerable<CellPosition> EnumeratePositions<T>(this T[,] field)
        {
            return
                from row in Enumerable.Range(0, field.GetHeight())
                from column in Enumerable.Range(0, field.GetWidth())
                select new CellPosition(row, column);
        }

        public static void Fill<T>(this T[,] field, Func<CellPosition, T> getValue)
        {
            foreach (var position in field.EnumeratePositions())
                field.SetValue(position, getValue(position));
        }
    }
}
