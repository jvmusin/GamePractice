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

        #region Base tests

        [Test]
        public void SayCorrectSize()
        {
            fieldSize = new Size(12, 2);
            builder = new GameFieldBuilder(new GameRules(fieldSize, new Dictionary<ShipType, int>()));

            builder.Size.Should().Be(fieldSize);
        }

        [Test]
        public void SayCorrectRules()
        {
            builder.Rules.Should().Be(rules);
        }

        #endregion

        #region Returning ShipsLeft counter tests

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

            builder.TryAddShipCell(new CellPosition(0, 0));
            builder.TryAddShipCell(new CellPosition(7, 2));
            builder.TryAddShipCell(new CellPosition(9, 7));

            builder.TryAddShipCell(new CellPosition(6, 4));
            builder.TryAddShipCell(new CellPosition(6, 5));

            builder.TryAddShipCell(new CellPosition(2, 1));
            builder.TryAddShipCell(new CellPosition(2, 2));

            foreach (var shipType in Enum.GetValues(typeof (ShipType)).Cast<ShipType>())
                shipsCount[shipType] = 0;

            builder.ShipsLeft.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void ReturnCorrectShipsLeftCounter_WhenTooManyShipsAdded()
        {
            for (var i = 0; i < 5; i++)
                builder.TryAddShipCell(new CellPosition(i*2, i*2));
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

        #endregion

        #region Building tests

        [Test]
        public void BuildAField_WhenEverythingIsOK()
        {
            shipsCount = new Dictionary<ShipType, int> {{ShipType.Submarine, 3}, {ShipType.Destroyer, 2}};
            rules = new GameRules(fieldSize, shipsCount);
            builder = new GameFieldBuilder(rules);

            builder.TryAddShipCell(new CellPosition(0, 0));
            builder.TryAddShipCell(new CellPosition(7, 2));
            builder.TryAddShipCell(new CellPosition(9, 7));

            builder.TryAddShipCell(new CellPosition(6, 4));
            builder.TryAddShipCell(new CellPosition(6, 5));

            builder.TryAddShipCell(new CellPosition(2, 1));
            builder.TryAddShipCell(new CellPosition(2, 2));

            foreach (var shipType in Enum.GetValues(typeof (ShipType)).Cast<ShipType>())
                shipsCount[shipType] = 0;

            builder.Build().Should().NotBeNull();
        }

        [Test]
        public void NotBuildAField_WhenThereAreUnusedShips()
        {
            shipsCount = new Dictionary<ShipType, int> {{ShipType.Submarine, 4}, {ShipType.Destroyer, 2}};
            rules = new GameRules(fieldSize, shipsCount);
            builder = new GameFieldBuilder(rules);

            builder.TryAddShipCell(new CellPosition(0, 0));
            builder.TryAddShipCell(new CellPosition(7, 2));
            builder.TryAddShipCell(new CellPosition(9, 7));

            builder.TryAddShipCell(new CellPosition(6, 4));
            builder.TryAddShipCell(new CellPosition(6, 5));

            builder.TryAddShipCell(new CellPosition(2, 1));
            builder.TryAddShipCell(new CellPosition(2, 2));

            foreach (var shipType in Enum.GetValues(typeof (ShipType)).Cast<ShipType>())
                shipsCount[shipType] = 0;

            builder.Build().Should().BeNull();
        }

        [Test]
        public void NotBuildAField_WhenThereAreTooManyShips()
        {
            shipsCount = new Dictionary<ShipType, int> {{ShipType.Submarine, 1}, {ShipType.Destroyer, 2}};
            rules = new GameRules(fieldSize, shipsCount);
            builder = new GameFieldBuilder(rules);

            builder.TryAddShipCell(new CellPosition(0, 0));
            builder.TryAddShipCell(new CellPosition(7, 2));
            builder.TryAddShipCell(new CellPosition(9, 7));

            builder.TryAddShipCell(new CellPosition(6, 4));
            builder.TryAddShipCell(new CellPosition(6, 5));

            builder.TryAddShipCell(new CellPosition(2, 1));
            builder.TryAddShipCell(new CellPosition(2, 2));

            foreach (var shipType in Enum.GetValues(typeof (ShipType)).Cast<ShipType>())
                shipsCount[shipType] = 0;

            builder.Build().Should().BeNull();
        }

        #endregion

        #region Connection tests

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

        [Test]
        public void SayThatItCanConnectManyCells()
        {
            for (var i = 0; i < 4; i++)
                builder.TryAddShipCell(new CellPosition(4, i)).Should().BeTrue();
        }

        [Test]
        public void SayThatItCantConnectTooManyCells_WhenConnectedSequenced()
        {
            var maxLen = Enum.GetValues(typeof (ShipType)).Cast<ShipType>().Max(x => x.GetLength());

            for (var i = 0; i < maxLen; i++)
                builder.TryAddShipCell(new CellPosition(4, i));
            builder.TryAddShipCell(new CellPosition(4, maxLen)).Should().BeFalse();
        }

        [Test]
        public void SayThatItCantConnectTooManyCells_WhenConnectedFromTwoSides()
        {
            var maxLen = Enum.GetValues(typeof (ShipType)).Cast<ShipType>().Max(x => x.GetLength());

            builder.TryAddShipCell(new CellPosition(0, 0));
            for (var i = 2; i <= maxLen; i++)
                builder.TryAddShipCell(new CellPosition(0, i));
            builder.TryAddShipCell(new CellPosition(0, 1)).Should().BeFalse();
        }

        [Test]
        public void ConnectACellOnField()
        {
            var position = new CellPosition(5, 6);
            builder.TryAddShipCell(position);
            builder[position].Should().BeTrue();
        }

        [Test]
        public void ConnectSomeCellsOnField()
        {
            var positions = new[] {new CellPosition(0, 1), new CellPosition(1, 1), new CellPosition(2, 1)};
            foreach (var position in positions)
                builder.TryAddShipCell(position);

            foreach (var position in positions)
                builder[position].Should().BeTrue();
        }

        [Test]
        public void NotConnectACellOutsideTheField()
        {
            builder.TryAddShipCell(new CellPosition(100, 500)).Should().BeFalse();
        }

        #endregion

        #region Disconnection tests

        [Test]
        public void SayThatItCantDisconnectAnEmptyCell()
        {
            builder.TryRemoveShipCell(new CellPosition(3, 5)).Should().BeFalse();
        }

        [Test]
        public void SayThatItDisconnectedALittleShipCorrectly()
        {
            var pos = new CellPosition(5, 6);
            builder.TryAddShipCell(pos);

            builder.TryRemoveShipCell(pos).Should().BeTrue();
        }

        [Test]
        public void SayThatItDisconnectedALargeShipCorrectly()
        {
            var positions = Enumerable.Range(0, 4).Select(i => new CellPosition(3, i)).ToList();
            foreach (var position in positions)
                builder.TryAddShipCell(position);
            builder.TryRemoveShipCell(positions[2]).Should().BeTrue();
        }

        [Test]
        public void DisconnectALittleShipOnField()
        {
            var pos = new CellPosition(5, 6);
            builder.TryAddShipCell(pos);

            builder.TryRemoveShipCell(pos);
            builder[pos].Should().BeFalse();
        }

        [Test]
        public void DisconnectALargeShipOnField()
        {
            var positions = Enumerable.Range(0, 4).Select(i => new CellPosition(3, i)).ToList();
            foreach (var position in positions)
                builder.TryAddShipCell(position);
            builder.TryRemoveShipCell(positions[2]).Should().BeTrue();

            builder[positions[0]].Should().BeTrue();
            builder[positions[1]].Should().BeTrue();
            builder[positions[2]].Should().BeFalse();
            builder[positions[3]].Should().BeTrue();
        }


        [Test]
        public void NotDisconnectACellOutsideTheField()
        {
            builder.TryRemoveShipCell(new CellPosition(100, 500)).Should().BeFalse();
        }

        #endregion

        #region Indexator tests

        [Test]
        public void ContainAnEmtyField_AfterCreation()
        {
            for (var row = 0; row < fieldSize.Height; row++)
                for (var column = 0; column < fieldSize.Width; column++)
                    builder[new CellPosition(row, column)].Should().BeFalse();
        }

        [Test]
        public void SayCorrectDataByIndexator()
        {
            var field = new[]
            {
                "--+++--++-",
                "----------",
                "-+---+----",
                "-----+----",
                "---+-+----",
                "---+---+++",
                "----------",
                "++++-----+",
                "---------+"
            };
            for (var row = 0; row < field.Length; row++)
                for (var column = 0; column < field.Length; column++)
                    if (field[row][column] == '+')
                        builder.TryAddShipCell(new CellPosition(row, column));

            for (var row = 0; row < field.Length; row++)
                for (var column = 0; column < field.Length; column++)
                    (field[row][column] == '+').Should().Be(builder[new CellPosition(row, column)]);
        }

        #endregion
    }
}
