using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Battleship.Base;
using Battleship.Implementations;
using Battleship.Interfaces;
using Battleship.Utilities;
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

        [Test]
        public void ClearFieldCorrectly()
        {
            for (var i = 0; i < 30; i++)
                builder.TryAddShipCell(CellPosition.Random(fieldSize));
            builder.Clear();
            builder.EnumeratePositions().Select(x => builder[x]).ShouldAllBeEquivalentTo(false);
        }

        #endregion

        #region Returning ShipsLeft counter tests

        [Test]
        public void ReturnCorrectShipsLeftCounter_WhenJustCreated()
        {
            builder.ShipsLeft.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void ReturnCorrectShipsLeftCounter_AfterAddingLittleShip()
        {
            builder.TryAddShipCell(new CellPosition(1, 3));
            shipsCount[(ShipType) 1]--;

            builder.ShipsLeft.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void ReturnCorrectShipsLeftCounter_AfterAddingSomeDifferentShips()
        {
            var cells = new[]
            {
                new CellPosition(1, 2),
                new CellPosition(2, 2),
                new CellPosition(3, 2),
                new CellPosition(4, 2),

                new CellPosition(1, 5),
                new CellPosition(1, 6),

                new CellPosition(9, 0),

                new CellPosition(9, 9),

                new CellPosition(6, 4),
                new CellPosition(6, 5)
            };

            foreach (var cell in cells)
                builder.TryAddShipCell(cell);
            shipsCount[(ShipType) 4]--;
            shipsCount[(ShipType) 2]--;
            shipsCount[(ShipType) 1]--;
            shipsCount[(ShipType) 1]--;
            shipsCount[(ShipType) 2]--;

            builder.ShipsLeft.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void ReturnCorrectShipsLeftCounter_WhenAllShipsUsed()
        {
            shipsCount = new Dictionary<ShipType, int> {{ShipType.Submarine, 3}, {ShipType.Destroyer, 2}};
            rules = new GameRules(fieldSize, shipsCount);
            builder = new GameFieldBuilder(rules);

            var cells = new[]
            {
                new CellPosition(0, 0),
                new CellPosition(7, 2),
                new CellPosition(9, 7),

                new CellPosition(6, 4),
                new CellPosition(6, 5),

                new CellPosition(2, 1),
                new CellPosition(2, 2)
            };
            foreach (var cell in cells)
                builder.TryAddShipCell(cell);

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
        public void ReturnCorrectShipsLeftCounter_AfterRemovingSomeCellsBetweenOthers()
        {
            var cells = new[]
            {
                new CellPosition(0, 1),
                new CellPosition(0, 2),
                new CellPosition(0, 3),
                new CellPosition(0, 4)
            };
            foreach (var cell in cells)
                builder.TryAddShipCell(cell);

            builder.TryRemoveShipCell(cells[1]);
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
        public void BuildField_WhenDataCorrect()
        {
            shipsCount = new Dictionary<ShipType, int> {{ShipType.Submarine, 3}, {ShipType.Destroyer, 2}};
            rules = new GameRules(fieldSize, shipsCount);
            builder = new GameFieldBuilder(rules);

            var cells = new[]
            {
                new CellPosition(0, 0),
                new CellPosition(7, 2),
                new CellPosition(9, 7),

                new CellPosition(6, 4),
                new CellPosition(6, 5),

                new CellPosition(2, 1),
                new CellPosition(2, 2)
            };
            foreach (var cell in cells)
                builder.TryAddShipCell(cell);

            builder.Build().Should().NotBeNull();
        }

        [Test]
        public void BuildCorrectField()
        {
            shipsCount = new Dictionary<ShipType, int> { { ShipType.Submarine, 3 }, { ShipType.Destroyer, 2 } };
            rules = new GameRules(fieldSize, shipsCount);
            builder = new GameFieldBuilder(rules);

            var cells = new[]
            {
                new CellPosition(0, 0),
                new CellPosition(7, 2),
                new CellPosition(9, 7),

                new CellPosition(6, 4),
                new CellPosition(6, 5),

                new CellPosition(2, 1),
                new CellPosition(2, 2)
            };
            foreach (var cell in cells)
                builder.TryAddShipCell(cell);

            foreach (var shipType in Enum.GetValues(typeof(ShipType)).Cast<ShipType>())
                shipsCount[shipType] = 0;

            var field = builder.Build();
            foreach (var cell in field.EnumeratePositions())
                cells.Contains(cell).Should().Be(field[cell] is IShipCell);
        }

        [Test]
        public void NotBuildField_WhenThereAreUnusedShips()
        {
            shipsCount = new Dictionary<ShipType, int> {{ShipType.Submarine, 4}, {ShipType.Destroyer, 2}};
            rules = new GameRules(fieldSize, shipsCount);
            builder = new GameFieldBuilder(rules);

            var cells = new[]
            {
                new CellPosition(0, 0),
                new CellPosition(7, 2),
                new CellPosition(9, 7),

                new CellPosition(6, 4),
                new CellPosition(6, 5),

                new CellPosition(2, 1),
                new CellPosition(2, 2)
            };
            foreach (var cell in cells)
                builder.TryAddShipCell(cell);

            foreach (var shipType in Enum.GetValues(typeof (ShipType)).Cast<ShipType>())
                shipsCount[shipType] = 0;

            builder.Build().Should().BeNull();
        }

        [Test]
        public void NotBuildField_WhenThereAreTooManyShips()
        {
            shipsCount = new Dictionary<ShipType, int> {{ShipType.Submarine, 1}, {ShipType.Destroyer, 2}};
            rules = new GameRules(fieldSize, shipsCount);
            builder = new GameFieldBuilder(rules);

            var cells = new[]
            {
                new CellPosition(0, 0),
                new CellPosition(7, 2),
                new CellPosition(9, 7),

                new CellPosition(6, 4),
                new CellPosition(6, 5),

                new CellPosition(2, 1),
                new CellPosition(2, 2)
            };
            foreach (var cell in cells)
                builder.TryAddShipCell(cell);

            foreach (var shipType in Enum.GetValues(typeof (ShipType)).Cast<ShipType>())
                shipsCount[shipType] = 0;

            builder.Build().Should().BeNull();
        }

        #endregion

        #region Adding tests

        [Test]
        public void SayThatItAddedCell()
        {
            builder.TryAddShipCell(new CellPosition(3, 4)).Should().BeTrue();
        }

        [Test]
        public void SayThatItCantAddAlreadyConnectedCell()
        {
            builder.TryAddShipCell(new CellPosition(3, 4));
            builder.TryAddShipCell(new CellPosition(3, 4)).Should().BeFalse();
        }

        [Test]
        public void SayThatItCantAddByVertexConnectedCells()
        {
            builder.TryAddShipCell(new CellPosition(3, 4));
            builder.TryAddShipCell(new CellPosition(4, 5)).Should().BeFalse();
        }

        [Test]
        public void SayThatItCanAddManyCells()
        {
            for (var i = 0; i < 4; i++)
                builder.TryAddShipCell(new CellPosition(4, i)).Should().BeTrue();
        }

        [Test]
        public void SayThatItCantAddTooManyCells_WhenConnectedSequenced()
        {
            var maxLen = Enum.GetValues(typeof (ShipType)).Cast<ShipType>().Max(x => x.GetLength());

            for (var i = 0; i < maxLen; i++)
                builder.TryAddShipCell(new CellPosition(4, i));
            builder.TryAddShipCell(new CellPosition(4, maxLen)).Should().BeFalse();
        }

        [Test]
        public void SayThatItCantAddTooManyCells_WhenConnectedFromBothSides()
        {
            var maxLen = Enum.GetValues(typeof (ShipType)).Cast<ShipType>().Max(x => x.GetLength());

            builder.TryAddShipCell(new CellPosition(0, 0));
            for (var i = 2; i <= maxLen; i++)
                builder.TryAddShipCell(new CellPosition(0, i));
            builder.TryAddShipCell(new CellPosition(0, 1)).Should().BeFalse();
        }

        [Test]
        public void AddCellOnField()
        {
            var position = new CellPosition(5, 6);
            builder.TryAddShipCell(position);
            builder[position].Should().BeTrue();
        }

        [Test]
        public void AddSomeCellsOnField()
        {
            var positions = new[] {new CellPosition(0, 1), new CellPosition(1, 1), new CellPosition(2, 1)};
            foreach (var position in positions)
                builder.TryAddShipCell(position);

            foreach (var position in positions)
                builder[position].Should().BeTrue();
        }

        [Test]
        public void NotAddCellOutsideTheField()
        {
            builder.TryAddShipCell(new CellPosition(100, 500)).Should().BeFalse();
        }

        [Test]
        public void ReturnTrue_OnCanBeAddedSafely_WhenCorrect()
        {
            var cells = new[]
            {
                new CellPosition(0, 0),
                new CellPosition(7, 2),
                new CellPosition(9, 7),

                new CellPosition(6, 4),
                new CellPosition(6, 5),

                new CellPosition(2, 1),
                new CellPosition(2, 2)
            };
            foreach (var cell in cells)
                builder.TryAddShipCell(cell);

            builder.CanBeAddedSafely(ShipType.Battleship, new CellPosition(0, 3), false).Should().BeTrue();
        }

        [Test]
        public void ReturnFalse_OnCanBeAddedSafely_WhenInorrect()
        {
            var cells = new[]
            {
                new CellPosition(0, 0),
                new CellPosition(7, 2),
                new CellPosition(9, 7),

                new CellPosition(6, 4),
                new CellPosition(6, 5),

                new CellPosition(2, 1),
                new CellPosition(2, 2)
            };
            foreach (var cell in cells)
                builder.TryAddShipCell(cell);

            builder.CanBeAddedSafely(ShipType.Battleship, new CellPosition(3, 3), false).Should().BeFalse();
        }

        [Test]
        public void AddFullShipsCorrectly()
        {
            var ships = new[]
            {
                new {Type = ShipType.Battleship, Start = new CellPosition(0, 0), Vertical = true},
                new {Type = ShipType.Cruiser, Start = new CellPosition(5, 2), Vertical = false},
                new {Type = ShipType.Submarine, Start = new CellPosition(9, 4), Vertical = true}
            };

            var cells = new[]
            {
                new CellPosition(0, 0),
                new CellPosition(1, 0),
                new CellPosition(2, 0),
                new CellPosition(3, 0),
                
                new CellPosition(5, 2),
                new CellPosition(5, 3),
                new CellPosition(5, 4),

                new CellPosition(9, 4)
            };

            foreach (var ship in ships)
                builder.TryAddFullShip(ship.Type, ship.Start, ship.Vertical).Should().BeTrue();

            foreach (var position in builder.EnumeratePositions())
                builder[position].Should().Be(cells.Contains(position));
        }

        [Test]
        public void NotAddFullShips_WhenIncorrect()
        {
            var ships = new[]
            {
                new {Type = ShipType.Battleship, Start = new CellPosition(0, 0), Vertical = true},
                new {Type = ShipType.Cruiser, Start = new CellPosition(0, 1), Vertical = false},
                new {Type = ShipType.Submarine, Start = new CellPosition(4, 1), Vertical = true}
            };

            foreach (var ship in ships)
                builder.TryAddFullShip(ship.Type, ship.Start, ship.Vertical).Should().Be(ship == ships[0]);

            var realCells = Enumerable.Range(0, 4).Select(row => new CellPosition(row, 0)).ToList();
            foreach (var position in builder.EnumeratePositions())
                builder[position].Should().Be(realCells.Contains(position));
        }

        [Test]
        public void RemoveFullShipsCorrectly()
        {
            var ships = new[]
            {
                new {Type = ShipType.Battleship, Start = new CellPosition(0, 0), Vertical = true},
                new {Type = ShipType.Cruiser, Start = new CellPosition(5, 5), Vertical = false},
                new {Type = ShipType.Submarine, Start = new CellPosition(9, 9), Vertical = true}
            };
            foreach (var ship in ships)
                builder.TryAddFullShip(ship.Type, ship.Start, ship.Vertical);

            foreach (var ship in ships)
                builder.TryRemoveFullShip(ship.Type, ship.Start, ship.Vertical).Should().BeTrue();

            foreach (var position in builder.EnumeratePositions())
                builder[position].Should().BeFalse();
        }

        [Test, TestCaseSource(nameof(RemovingShipsTestsWhichShouldFail))]
        public void NotRemoveFullShip_WhenDataIncorrect(
            ShipType shipType, CellPosition start, bool vertical,
            ShipType toRemoveShipType, CellPosition toRemoveStart, bool toRemoveVertical)
        {
            builder.TryAddFullShip(shipType, start, vertical);

            builder.TryRemoveFullShip(toRemoveShipType, toRemoveStart, toRemoveVertical).Should().BeFalse();

            var realCells = GenerateShipCells(shipType, start, vertical).ToList();
            foreach (var position in builder.EnumeratePositions())
                builder[position].Should().Be(realCells.Contains(position));
        }

        private static IEnumerable<TestCaseData> RemovingShipsTestsWhichShouldFail
        {
            get
            {
                yield return new TestCaseData(
                    ShipType.Battleship, new CellPosition(3, 3), true,
                    ShipType.Battleship, new CellPosition(2, 3), true);
                yield return new TestCaseData(
                    ShipType.Battleship, new CellPosition(3, 3), true,
                    ShipType.Battleship, new CellPosition(4, 3), true);
                yield return new TestCaseData(
                    ShipType.Battleship, new CellPosition(3, 3), true,
                    ShipType.Cruiser, new CellPosition(3, 3), true);
                yield return new TestCaseData(
                    ShipType.Submarine, new CellPosition(9, 9), true,
                    ShipType.Battleship, new CellPosition(9, 9), true);
                yield return new TestCaseData(
                    ShipType.Battleship, new CellPosition(3, 3), true,
                    ShipType.Battleship, new CellPosition(3, 3), false);
            }
        }

        [Test]
        public void AddNewShipsAtPlace_WhereAnotherShipHasBeenRemoved()
        {
            var ship = new {Type = ShipType.Battleship, Start = new CellPosition(0, 0), Vertical = true};

            builder.TryAddFullShip(ship.Type, ship.Start, ship.Vertical);
            builder.TryRemoveFullShip(ship.Type, ship.Start, ship.Vertical);
            builder.TryAddFullShip(ship.Type, ship.Start, ship.Vertical).Should().BeTrue();

            var realCells = GenerateShipCells(ship.Type, ship.Start, ship.Vertical).ToList();
            foreach (var position in builder.EnumeratePositions())
                builder[position].Should().Be(realCells.Contains(position));
        }

        private static IEnumerable<CellPosition> GenerateShipCells(
            ShipType ship, CellPosition start, bool vertical)
        {
            var delta = vertical ? CellPosition.DeltaDown : CellPosition.DeltaRight;
            for (var i = 0; i < ship.GetLength(); i++)
                yield return start + delta * i;
        }

        #endregion

        #region Removing tests

        [Test]
        public void SayThatItCantRemoveEmptyCell()
        {
            builder.TryRemoveShipCell(new CellPosition(3, 5)).Should().BeFalse();
        }

        [Test]
        public void SayThatItRemovedLittleShipCorrectly()
        {
            var pos = new CellPosition(5, 6);
            builder.TryAddShipCell(pos);

            builder.TryRemoveShipCell(pos).Should().BeTrue();
        }

        [Test]
        public void SayThatItRemovedLargeShipCorrectly()
        {
            var positions = Enumerable.Range(0, 4).Select(i => new CellPosition(3, i)).ToList();
            foreach (var position in positions)
                builder.TryAddShipCell(position);
            builder.TryRemoveShipCell(positions[2]).Should().BeTrue();
        }

        [Test]
        public void RemoveLittleShipCorrectly()
        {
            var pos = new CellPosition(5, 6);
            builder.TryAddShipCell(pos);

            builder.TryRemoveShipCell(pos);
            builder[pos].Should().BeFalse();
        }

        [Test]
        public void DisconnectLargeShipCorrectly()
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
        public void NotDisconnectCellOutsideTheField()
        {
            builder.TryRemoveShipCell(new CellPosition(100, 500)).Should().BeFalse();
        }

        #endregion

        #region Indexer tests

        [Test]
        public void ContainEmptyField_WhenJustCreated()
        {
            for (var row = 0; row < fieldSize.Height; row++)
                for (var column = 0; column < fieldSize.Width; column++)
                    builder[new CellPosition(row, column)].Should().BeFalse();
        }

        [Test]
        public void SayCorrectDataByIndexer()
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
                    builder[new CellPosition(row, column)].Should().Be(field[row][column] == '+');
        }

        #endregion
    }
}
