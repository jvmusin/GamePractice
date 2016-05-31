using System;
using System.Collections.Generic;
using System.Linq;
using Battleship.Implementations;
using Battleship.Interfaces;

namespace Battleship.Utilities
{
    public static class RectangularReadonlyFieldExtensions
    {
        public static int GetHeight<T>(this IRectangularReadonlyField<T> field)
        {
            return field.Size.Height;
        }

        public static int GetWidth<T>(this IRectangularReadonlyField<T> field)
        {
            return field.Size.Width;
        }

        public static bool IsOnField<T>(this IRectangularReadonlyField<T> field, CellPosition cell)
        {
            return
                cell.Row.IsInRange(0, field.Size.Height) &&
                cell.Column.IsInRange(0, field.Size.Width);
        }
        
        public static IEnumerable<CellPosition> EnumeratePositions<T>(this IRectangularReadonlyField<T> field)
        {
            return
                from row in Enumerable.Range(0, field.GetHeight())
                from column in Enumerable.Range(0, field.GetWidth())
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
                    .Where(x => field.IsOnField(x) && !visited.Contains(x) && canBeVisited(field[x]));
                foreach (var connected in ways)
                {
                    visited.Add(connected);
                    queue.Enqueue(connected);
                }
            }

            return visited;
        }
    }
}
