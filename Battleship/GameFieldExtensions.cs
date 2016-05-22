using System.Collections.Generic;
using System.Linq;

namespace Battleship
{
    public static class GameFieldExtensions
    {
        public static IEnumerable<CellPosition> EnumerateCellPositions(this IBattleshipGameField field)
        {
            return
                from row in Enumerable.Range(0, field.Height)
                from column in Enumerable.Range(0, field.Width)
                select new CellPosition(row, column);
        }

        public static IEnumerable<IGameCell> GetRow(this IBattleshipGameField field, int row)
        {
            return Enumerable.Range(0, field.Width)
                .Select(column => field.GetElementAt(row, column));
        }

        public static IEnumerable<IGameCell> GetColumn(this IBattleshipGameField field, int column)
        {
            return Enumerable.Range(0, field.Height)
                .Select(row => field.GetElementAt(row, column));
        }

        public static bool IsOnField(this IBattleshipGameField field, int row, int column)
        {
            var height = field.Height;
            var width = field.Width;
            return
                0 <= row && row < height &&
                0 <= column && column < width;
        }

        public static IEnumerable<CellPosition> Get8Neighbours(this IBattleshipGameField field, int row, int column)
        {
            var deltas = new[] {-1, 0, 1};
            return
                from deltaRow in deltas
                from deltaColumn in deltas
                where !(deltaRow == 0 && deltaColumn == 0)
                let position = new CellPosition(row + deltaRow, column + deltaColumn)
                where field.IsOnField(position)
                select position;
        }

        public static IEnumerable<CellPosition> GetDiagonalNeighbours(
            this IBattleshipGameField field, int row, int column)
        {
            var deltas = new[] {-1, 1};
            return 
                from deltaRow in deltas
                from deltaColumn in deltas
                let position = new CellPosition(row + deltaRow, column + deltaColumn)
                where field.IsOnField(position)
                select position;
        }

        #region Using CellPosition utility class

        public static IGameCell GetElementAt(this IBattleshipGameField field, CellPosition position)
        {
            return field.GetElementAt(position.Row, position.Column);
        }

        public static bool IsOnField(this IBattleshipGameField field, CellPosition position)
        {
            return field.IsOnField(position.Row, position.Column);
        }

        public static IEnumerable<CellPosition> Get8Neighbours(this IBattleshipGameField field, CellPosition position)
        {
            return field.Get8Neighbours(position.Row, position.Column);
        }

        public static IEnumerable<CellPosition> GetDiagonalNeighbours(
            this IBattleshipGameField field, CellPosition position)
        {
            return field.GetDiagonalNeighbours(position.Row, position.Column);
        }

        #endregion
    }
}
