using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Battleship.Implementations
{
    public class GameRules
    {
        public Size FieldSize { get; }
        public IReadOnlyDictionary<ShipType, int> ShipsCount { get; }

        public GameRules(Size fieldSize, IReadOnlyDictionary<ShipType, int> shipsCount)
        {
            if (shipsCount == null)
                throw new ArgumentNullException(nameof(shipsCount));

            if (fieldSize.Height < 1 || fieldSize.Width < 1)
                throw new ArgumentOutOfRangeException(nameof(fieldSize));

            FieldSize = fieldSize;
            ShipsCount = AddMissingKeys(shipsCount);
        }

        private static IReadOnlyDictionary<ShipType, int> AddMissingKeys(IReadOnlyDictionary<ShipType, int> counter)
        {
            var missedTypes = Enum.GetValues(typeof (ShipType))
                .Cast<ShipType>()
                .Where(shipType => !counter.ContainsKey(shipType))
                .ToList();
            if (missedTypes.Any())
            {
                var newCounter = counter.ToDictionary(x => x.Key, x => x.Value);
                foreach (var type in missedTypes)
                    newCounter[type] = 0;
                counter = newCounter;
            }
            return counter;
        }

        public static GameRules Default
        {
            get
            {
                var fieldSize = new Size(10, 10);
                var shipCount = Enum.GetValues(typeof (ShipType))
                    .Cast<ShipType>()
                    .ToDictionary(x => x, x => 5 - x.GetLength());
                return new GameRules(fieldSize, shipCount);
            }
        }
    }
}
