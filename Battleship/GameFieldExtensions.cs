using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    public static class GameFieldExtensions
    {
        public static IGameField<T> Fill<T>(this IGameField<T> field, Func<int, int, T> getNumber)
        {
            foreach (var position in field.EnumerateCellPositions())
            {
                var row = position.Row;
                var column = position.Column;
                var number = getNumber(row, column);
                field = field.SetElementAt(row, column, number);
            }
            return field;
        }

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
        
        public static IGameField<T> SetElementAt<T>(this IGameField<T> field, CellPosition position, T value)
        {
            return field.SetElementAt(position.Row, position.Column, value);
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
