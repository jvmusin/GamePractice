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
            ShipsCount = AddMissingTypes(shipsCount);
        }

        private static IReadOnlyDictionary<ShipType, int> AddMissingTypes(IReadOnlyDictionary<ShipType, int> counter)
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
                    .ToDictionary(x => x, x => (x == ShipType.None) ? 0 : 5 - x.GetLength());
                return new GameRules(fieldSize, shipCount);
            }
        }

        public static GameRules EmptyField => new GameRules(new Size(10, 10), new Dictionary<ShipType, int>());

        protected bool Equals(GameRules other)
        {
            return 
                Equals(FieldSize, other.FieldSize) && 
                Equals(ShipsCount, other.ShipsCount);
        }

        public override bool Equals(object obj)
        {
            var other = obj as GameRules;
            return other != null & Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FieldSize.GetHashCode()*397) ^ (ShipsCount?.GetHashCode() ?? 0);
            }
        }
    }
}
