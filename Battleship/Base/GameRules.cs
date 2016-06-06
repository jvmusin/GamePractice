using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Battleship.Implementations;

namespace Battleship.Base
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
                throw new ArgumentOutOfRangeException(nameof(fieldSize), fieldSize, "Field borders should have positive length");

            FieldSize = fieldSize;
            ShipsCount = AddMissingTypes(shipsCount);
        }

        private static IReadOnlyDictionary<ShipType, int> AddMissingTypes(IReadOnlyDictionary<ShipType, int> counter)
        {
            var missedTypes = Enum.GetValues(typeof(ShipType))
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
                var shipsCount = Enum.GetValues(typeof(ShipType))
                    .Cast<ShipType>()
                    .ToDictionary(x => x, x => 5 - x.GetLength());
                return new GameRules(fieldSize, shipsCount);
            }
        }

        private bool EqualsShipsCount(IReadOnlyDictionary<ShipType, int> other)
        {
            return ShipsCount.Keys.All(k => ShipsCount[k] == other[k]);
        }

        protected bool Equals(GameRules other)
        {
            return
                Equals(FieldSize, other.FieldSize) &&
                EqualsShipsCount(other.ShipsCount);
        }

        public override bool Equals(object obj)
        {
            var other = obj as GameRules;
            return other != null & Equals(other);
        }

        public override int GetHashCode()
            => FieldSize.GetHashCode();
    }
}
