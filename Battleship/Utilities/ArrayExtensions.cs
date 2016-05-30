using System;
using System.Collections.Generic;
using System.Linq;
using Battleship.Implementations;

namespace Battleship.Utilities
{
    public static class ArrayExtensions
    {
        public static void Fill<T>(this T[,] field, T value)
        {
            field.Fill(x => value);
        }

        public static void Fill<T>(this T[,] field, Func<CellPosition, T> getValue)
        {
            foreach (var position in field.EnumeratePositions())
                field.SetValue(getValue(position), position);
        }

        public static IEnumerable<CellPosition> EnumeratePositions<T>(this T[,] field)
        {
            return 
                from row in Enumerable.Range(0, field.GetLength(0))
                from column in Enumerable.Range(0, field.GetLength(1))
                select new CellPosition(row, column);
        }

        public static void SetValue<T>(this T[,] field, T value, CellPosition position)
        {
            field[position.Row, position.Column] = value;
        }

        public static T GetValue<T>(this T[,] field, CellPosition position)
        {
            return field[position.Row, position.Column];
        }
    }
}
