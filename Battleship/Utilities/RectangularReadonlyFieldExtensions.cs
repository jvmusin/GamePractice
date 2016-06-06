using System;
using System.Collections.Generic;
using System.Linq;
using Battleship.Base;
using Battleship.Interfaces;

namespace Battleship.Utilities
{
    public static class RectangularReadonlyFieldExtensions
    {
        public static bool Contains<T>(this IRectangularReadonlyField<T> field, CellPosition cell)
        {
            return
                cell.Row.IsInRange(0, field.Size.Height) &&
                cell.Column.IsInRange(0, field.Size.Width);
        }
        
        public static IEnumerable<CellPosition> EnumeratePositions<T>(this IRectangularReadonlyField<T> field)
        {
            return
                from row in Enumerable.Range(0, field.Size.Height)
                from column in Enumerable.Range(0, field.Size.Width)
                select new CellPosition(row, column);
        }

        public static IEnumerable<CellPosition> FindAllConnectedByEdgeCells<T>(
            this IRectangularReadonlyField<T> field, CellPosition start, Predicate<T> canBeVisited)
        {
            var visited = new HashSet<CellPosition> { start };
            var queue = new Queue<CellPosition>();
            queue.Enqueue(start);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                var ways = current.ByEdgeNeighbours
                    .Where(x => field.Contains(x) && !visited.Contains(x) && canBeVisited(field[x]));
                foreach (var connected in ways)
                {
                    visited.Add(connected);
                    queue.Enqueue(connected);
                }
            }

            return visited;
        }

        public static string ToString<T>(this IRectangularReadonlyField<T> field, Func<T, char> getSymbol)
        {
            var rows = Enumerable.Range(0, field.Size.Height)
                .Select(x => new char[field.Size.Width])
                .ToArray();
            foreach (var position in field.EnumeratePositions())
                rows[position.Row][position.Column] = getSymbol(field[position]);
            return string.Join("\n", rows.Select(row => new string(row)));
        }
    }
}
