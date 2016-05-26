using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Battleship.Implementations;
using Battleship.Interfaces;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GameField_Should
    {
        private GameRules rules;
        private Dictionary<ShipType, int> shipsCount;
        private IGameField field;

        private Size FieldSize => rules.FieldSize;

        [SetUp]
        public void SetUp()
        {
            rules = GameRules.Default;
            shipsCount = rules.ShipsCount.ToDictionary(x => x.Key, x => x.Value);
            field = FromLines(rules, SampleField);
        }

        #region Base tests

        [Test]
        public void ReturnCorrectRules()
        {
            field.Rules.Should().Be(rules);
        }

        [Test]
        public void ReturnCorrectSize()
        {
            field.Size.Should().Be(FieldSize);
        }

        #endregion

        #region Survived ships tests

        [Test]
        public void ReturnCorrectSurvivedShips_AfterCreating()
        {
            field.SurvivedShips.Should().BeEquivalentTo(rules.ShipsCount);
        }

        [Test]
        public void DecreaseSurvivedShips_AfterKilling()
        {
            var row = 4;
            var startColumn = 3;
            var length = 3;

            foreach (var column in Enumerable.Range(startColumn, length))
                field.Shoot(new CellPosition(row, column));
            shipsCount[(ShipType)length]--;

            field.SurvivedShips.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void NotDecreaseSurvivedShips_AfterHitWithoutKilling()
        {
            field.Shoot(new CellPosition(2, 4));

            field.SurvivedShips.Should().BeEquivalentTo(shipsCount);
        }

        [Test]
        public void NotDecreaseSurvivedShips_AfterMissing()
        {
            field.Shoot(new CellPosition(5, 4));

            field.SurvivedShips.Should().BeEquivalentTo(shipsCount);
        }

        #endregion

        #region Field building

        private static readonly string[] SampleField = {
            ".X..X...X.",
            ".X........",
            "...XXXX...",
            "..........",
            "X..XXX....",
            ".......X..",
            "...X...X..",
            ".......X..",
            "XX........",
            "........XX"
        };


        private static IGameField FromLines(GameRules rules, string[] lines)
        {
            var builder = new GameFieldBuilder(rules);
            for (var row = 0; row < rules.FieldSize.Height; row++)
                for (var column = 0; column < rules.FieldSize.Width; column++)
                    if (lines[row][column] == 'X')
                        builder.TryAddShipCell(new CellPosition(row, column));
            return builder.Build();
        }

        #endregion
    }
}
