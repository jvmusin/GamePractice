using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Battleship.Implementations;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GameFieldBuilder_Should
    {
        private Size fieldSize;
        private Dictionary<ShipType, int> shipsCount;
        private GameRules rules;
        private GameFieldBuilder builder;

        [SetUp]
        public void SetUp()
        {
            rules = GameRules.Default;
            fieldSize = rules.FieldSize;
            shipsCount = rules.ShipsCount.ToDictionary(x => x.Key, x => x.Value);
            builder = new GameFieldBuilder(rules);
        }

        [Test]
        public void SayCorrectSize()
        {
            fieldSize = new Size(12, 2);
            builder = new GameFieldBuilder(new GameRules(fieldSize, new Dictionary<ShipType, int>()));

            builder.FieldSize.Should().Be(fieldSize);
        }

        [Test]
        public void ReturnCorrectShipsLeftCounter_WhenJustCreated()
        {
            builder.ShipsLeft.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void ReturnCorrectShipsLeftCounter_AfterAddingALittleShip()
        {
            builder.TryAddShipCell(new CellPosition(1, 3));
            shipsCount[(ShipType) 1]--;

            builder.ShipsLeft.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void ReturnCorrectShipsLeftCounter_AfterAddingSomeDifferentShips()
        {
            builder.TryAddShipCell(new CellPosition(1, 2));
            builder.TryAddShipCell(new CellPosition(2, 2));
            builder.TryAddShipCell(new CellPosition(3, 2));
            builder.TryAddShipCell(new CellPosition(4, 2));
            shipsCount[(ShipType) 4]--;

            builder.TryAddShipCell(new CellPosition(1, 5));
            builder.TryAddShipCell(new CellPosition(1, 6));
            shipsCount[(ShipType) 2]--;

            builder.TryAddShipCell(new CellPosition(9, 0));
            shipsCount[(ShipType) 1]--;

            builder.TryAddShipCell(new CellPosition(9, 9));
            shipsCount[(ShipType) 1]--;

            builder.TryAddShipCell(new CellPosition(6, 4));
            builder.TryAddShipCell(new CellPosition(6, 5));
            shipsCount[(ShipType) 2]--;

            builder.ShipsLeft.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void ReturnCorrectShipsLeftCounter_WhenAllShipsUsed()
        {
            shipsCount = new Dictionary<ShipType, int> {{ShipType.Submarine, 3}, {ShipType.Destroyer, 2}};
            rules = new GameRules(fieldSize, shipsCount);
            builder = new GameFieldBuilder(rules);

            builder.TryAddShipCell(new CellPosition(7, 2));
            builder.TryAddShipCell(new CellPosition(0, 0));
            builder.TryAddShipCell(new CellPosition(9, 7));

            builder.TryAddShipCell(new CellPosition(6, 4));
            builder.TryAddShipCell(new CellPosition(6, 5));
            
            builder.TryAddShipCell(new CellPosition(2, 1));
            builder.TryAddShipCell(new CellPosition(2, 2));

            foreach (var shipType in Enum.GetValues(typeof(ShipType)).Cast<ShipType>())
                shipsCount[shipType] = 0;

            builder.ShipsLeft.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void ReturnCorrectShipsLeftCounter_WhenTooManyShipsAdded()
        {
            for (var i = 0; i < 5; i ++)
                builder.TryAddShipCell(new CellPosition(i * 2, i * 2));
            shipsCount[(ShipType) 1] -= 5;

            builder.ShipsLeft.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void ReturnCorrectShipsLeftCounter_AfterRemovingCells()
        {
            builder.TryAddShipCell(new CellPosition(0, 1));
            builder.TryAddShipCell(new CellPosition(0, 2));
            builder.TryAddShipCell(new CellPosition(0, 3));
            builder.TryAddShipCell(new CellPosition(0, 4));

            builder.TryRemoveShipCell(new CellPosition(0, 2));

            shipsCount[(ShipType) 1]--;
            shipsCount[(ShipType) 2]--;

            builder.ShipsLeft.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void ReturnCorrectShipsLeftCounter_AfterRemovingAllCells()
        {
            var cells = new[]
            {
                new CellPosition(0, 1),
                new CellPosition(0, 2),
                new CellPosition(0, 3),
                new CellPosition(0, 4),

                new CellPosition(5, 5),
                new CellPosition(5, 6),
                
                new CellPosition(8, 9)   
            };

            foreach (var cell in cells)
                builder.TryAddShipCell(cell);
            foreach (var cell in cells)
                builder.TryRemoveShipCell(cell);

            builder.ShipsLeft.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void SayThatItConnectedACell()
        {
            builder.TryAddShipCell(new CellPosition(3, 4)).Should().BeTrue();
        }

        [Test]
        public void SayThatItCantConnectAlreadyConnectedCell()
        {
            builder.TryAddShipCell(new CellPosition(3, 4));
            builder.TryAddShipCell(new CellPosition(3, 4)).Should().BeFalse();
        }

        [Test]
        public void SayThatItCantConnectByAngleConnectedCells()
        {
            builder.TryAddShipCell(new CellPosition(3, 4));
            builder.TryAddShipCell(new CellPosition(4, 5)).Should().BeFalse();
        }
    }
}
