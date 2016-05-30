using System;
using System.Collections.Generic;
using System.Linq;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class RandomFieldGenerator
    {
        private readonly IGameFieldBuilder builder;
        private readonly Random rnd = new Random();

        public RandomFieldGenerator(IGameFieldBuilder builder)
        {
            this.builder = builder;
        }

        public IGameField Generate()
        {
            builder.Clear();
            var allShips = builder.Rules.ShipsCount.SelectMany(x => Enumerable.Repeat(x.Key, x.Value)).ToList();
            if (!TryAddAllShips(allShips))
                throw new InvalidOperationException("Field can't be built");
            return builder.Build();
        }

        private bool TryAddAllShips(IList<ShipType> ships)
        {
            if (!ships.Any())
                return true;

            var ship = ships.Last();
            ships.RemoveAt(ships.Count - 1);

            var availablePlaces = builder.EnumerateCellPositions()
                .SelectMany(position => new[]
                {
                    new
                    {
                        Position = position,
                        Vertical = true
                    },
                    new
                    {
                        Position = position,
                        Vertical = false
                    }
                })
                .Where(x => CanBeAdded(ship, x.Position, x.Vertical))
                .OrderBy(x => rnd.Next()).ToList();

            foreach (var place in availablePlaces)
            {
                AddFullShip(ship, place.Position, place.Vertical);
                if (TryAddAllShips(ships))
                    return true;
                RemoveFullShip(ship, place.Position, place.Vertical);
            }

            ships.Add(ship);
            return false;
        }

        private bool CanBeAdded(ShipType ship, CellPosition startPosition, bool vertical)
        {
            return EnumerateShipPositions(startPosition, vertical, ship)
                .All(position =>
                    builder.IsOnField(position) && !builder[position] &&
                    !position.AllNeighbours.Any(x => builder.IsOnField(x) && builder[x]));
        }

        private void AddFullShip(ShipType ship, CellPosition startPosition, bool vertical)
        {
            foreach (var position in EnumerateShipPositions(startPosition, vertical, ship))
                if (!builder.TryAddShipCell(position))
                    throw null;
        }

        private void RemoveFullShip(ShipType ship, CellPosition startPosition, bool vertical)
        {
            foreach (var position in EnumerateShipPositions(startPosition, vertical, ship))
                if (!builder.TryRemoveShipCell(position))
                    throw null;
        }

        private static IEnumerable<CellPosition> EnumerateShipPositions(
            CellPosition startPosition, bool vertical, ShipType ship)
        {
            var delta = vertical ? CellPosition.DeltaDown : CellPosition.DeltaRight;
            for (var i = 0; i < ship.GetLength(); i++, startPosition += delta)
                yield return startPosition;
        }
    }
}
