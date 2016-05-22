using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SudokuSolver;

namespace Battleship.Tests
{
    [TestFixture]
    public class BattleshipGameField_Should : TestBase
    {
        private IGameField<Ship> field;
        private Func<int, int, Ship> byRowNumerator;

        [SetUp]
        public void SetUp()
        {
            field = new BattleshipGameField(5, 6);
            byRowNumerator = (row, column) => new Ship(ShipType.Submarine, row*field.Width + column);
        }

        [Test]
        public void SaySizeCorrectly_WhenSizeIsCorrect()
        {
            var height = 5;
            var width = 6;
            field = new BattleshipGameField(height, width);

            field.Height.Should().Be(height);
            field.Width.Should().Be(width);
        }

        [Test, Combinatorial]
        public void FailCreating_WhenSizeIsNotCorrect(
            [Values(-6, 0, 6)] int height,
            [Values(-5, 0, 5)] int width)
        {
            if (height > 0 && width > 0)
                return;
            Action create = () => new BattleshipGameField(height, width);
            create.ShouldThrow<Exception>();
        }

        [Test]
        public void RememberCellValuesCorrectly()
        {
            field = (BattleshipGameField) field.Fill(byRowNumerator);

            foreach (var position in field.EnumerateCellPositions())
            {
                var row = position.Row;
                var column = position.Column;
                field.GetElementAt(row, column).Should().Be(byRowNumerator(row, column));
            }
        }

        [Test]
        public void ReturnRealValuesOnGetRow()
        {
            field = (BattleshipGameField) field.Fill(byRowNumerator);

            foreach (var row in Enumerable.Range(0, field.Height))
            {
                var rowValues = field.GetRow(row).ToList();
                rowValues.Count.Should().Be(field.Width);
                foreach (var column in Enumerable.Range(0, field.Width))
                    rowValues[column].Should().Be(byRowNumerator(row, column));
            }
        }

        [Test]
        public void ReturnRealValuesOnGetColumn()
        {
            field = (BattleshipGameField) field.Fill(byRowNumerator);

            foreach (var column in Enumerable.Range(0, field.Width))
            {
                var columnValues = field.GetColumn(column).ToList();
                columnValues.Count.Should().Be(field.Height);
                foreach (var row in Enumerable.Range(0, field.Height))
                    columnValues[row].Should().Be(byRowNumerator(row, column));
            }
        }

        [Test]
        public void HaveNullValues_ByDefault()
        {
            Enumerable.Range(0, field.Height)
                .SelectMany(row => field.GetRow(row))
                .ShouldAllBeEquivalentTo((Ship) null);
        }

        [Test]
        public void ReturnBattleshipGameFieldClass_OnSetElementAtMethod()
        {
            field.SetElementAt(0, 0, null).GetType().Should().Be<BattleshipGameField>();
        }
    }
}
