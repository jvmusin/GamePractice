using System.Collections.Generic;
using System.Linq;

namespace Battleship
{
    public interface IBattleshipGameField : IRectangleReadonlyField<IGameCell>
    {
        void Put(Ship ship, int row, int column, bool vertical);
        bool Shoot(int row, int column);
        bool IsAvailablePositionFor(ShipType type, int row, int column, bool vertical);
    }

    public static class BattleshipGameFieldExtensions
    {
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
