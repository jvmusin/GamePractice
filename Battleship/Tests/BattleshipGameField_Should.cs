//using System;
//using System.Linq;
//using Battleship.Implementations;
//using Battleship.Interfaces;
//using Battleship.Utilities;
//using FluentAssertions;
//using NUnit.Framework;
//
//namespace Battleship.Tests
//{
//    [TestFixture]
//    public class BattleshipGameField_Should : TestBase
//    {
//        private GameField field;
//        private Func<CellPosition, IGameCell> byRowNumerator;
//
//        [SetUp]
//        public void SetUp()
//        {
//            field = new GameField(10, 15);
//            byRowNumerator = (row, column) => new GameCell(CellType.Empty);
//        }
//
//        [Test]
//        public void SaySizeCorrectly_WhenSizeIsCorrect()
//        {
//            var height = 5;
//            var width = 6;
//            field = new GameField(height, width);
//
//            field.Height.Should().Be(height);
//            field.Width.Should().Be(width);
//        }
//
//        [Test, Combinatorial]
//        public void FailCreating_WhenSizeIsNotCorrect(
//            [Values(-6, 0, 6)] int height,
//            [Values(-5, 0, 5)] int width)
//        {
//            if (height > 0 && width > 0)
//                return;
//            Action create = () => new GameField(height, width);
//            create.ShouldThrow<Exception>();
//        }
//
//        [Test]
//        public void RememberCellValuesCorrectly()
//        {
//            field = new GameField(10, 15, byRowNumerator);
//
//            foreach (var position in field.EnumerateCellPositions())
//            {
//                var row = position.Row;
//                var column = position.Column;
//                field[position].Should().Be(byRowNumerator(row, column));
//            }
//        }
//
//        [Test]
//        public void ReturnRealValuesOnGetRow()
//        {
//            field = new GameField(10, 15, byRowNumerator);
//
//            foreach (var row in Enumerable.Range(0, field.Height))
//            {
//                var rowValues = field.GetRow(row).ToList();
//                rowValues.Count.Should().Be(field.Width);
//                foreach (var column in Enumerable.Range(0, field.Width))
//                    rowValues[column].Should().Be(byRowNumerator(row, column));
//            }
//        }
//
//        [Test]
//        public void ReturnRealValuesOnGetColumn()
//        {
//            field = new GameField(10, 15, byRowNumerator);
//
//            foreach (var column in Enumerable.Range(0, field.Width))
//            {
//                var columnValues = field.GetColumn(column).ToList();
//                columnValues.Count.Should().Be(field.Height);
//                foreach (var row in Enumerable.Range(0, field.Height))
//                    columnValues[row].Should().Be(byRowNumerator(row, column));
//            }
//        }
//
//        [Test]
//        public void BeEmptyEverywhere_ByDefault()
//        {
//            Enumerable.Range(0, field.Height)
//                .SelectMany(row => field.GetRow(row))
//                .ShouldAllBeEquivalentTo(new GameCell(CellType.Empty));
//        }
//
//        [Test]
//        public void LetMePutAHorizontalLongShipOnTheField()
//        {
//            field.IsAvailablePositionFor(ShipType.Battleship, 3, 11, false);
//        }
//
//        [Test]
//        public void LetMePutAVerticalLongShipOnTheField()
//        {
//            field.IsAvailablePositionFor(ShipType.Battleship, 5, 11, true);
//        }
//
//        [Test]
//        public void NotLetMePutTwoShips_CrossingOneByAnother()
//        {
//            var ship = new Ship(ShipType.Battleship);
//            field.Put(ship, 3, 3, false);
//            field.IsAvailablePositionFor(ShipType.Battleship, 2, 4, true).Should().BeFalse();
//        }
//
//        [Test]
//        public void NotLetMePutTwoShips_WhenFirstTouchesSecond()
//        {
//            var ship1 = new Ship(ShipType.Battleship);
//            var ship2 = new Ship(ShipType.Battleship);
//            field.Put(ship1, 0, 0, true);
//            field.IsAvailablePositionFor(ship2.Type, 4, 1, true).Should().BeFalse();
//        }
//
//        [Test]
//        public void NotLetMePutAShip_IfItDoesntFit()
//        {
//            field.IsAvailablePositionFor(ShipType.Battleship, 9, 11, true).Should().BeFalse();
//        }
//    }
//}
