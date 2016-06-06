using System;
using System.Collections.Generic;
using System.Linq;
using Battleship.Base;
using Battleship.Implementations;
using Battleship.Interfaces;
using Battleship.Utilities;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class RandomFieldGenerator_Should
    {
        private static readonly GameRules Rules = GameRules.Default;
        private IGameFieldBuilder builder;
        private RandomFieldGenerator generator;

        [SetUp]
        public void SetUp()
        {
            builder = FromLines(Rules, SampleField);
            generator = new RandomFieldGenerator(builder);
        }

        [Test]
        public void NotRemoveAlreadyExistedShips()
        {
            var field = generator.Generate();
            foreach (var position in field.EnumeratePositions())
                if (SampleField[position.Row][position.Column] == 'X')
                    field[position].Should().BeAssignableTo<IShipCell>();
        }

        [Test]
        public void GenerateFilledGameField()
        {
            var field = generator.Generate();
            var shipCellsDone = field.EnumeratePositions()
                .Select(x => field[x])
                .OfType<IShipCell>()
                .Select(x => x.Ship.Type)
                .GroupBy(x => x, (key, values) => new {ShipType = key, CellsCount = values.Count()});

            foreach (var group in shipCellsDone)
            {
                if (group.CellsCount % group.ShipType.GetLength() != 0)
                    throw new AssertionException("Some ships are not full");
                var shipsCount = group.CellsCount/group.ShipType.GetLength();
                shipsCount.Should().Be(Rules.ShipsCount[group.ShipType]);
            }
        }

        [Test]
        public void NotModifyBuilder_BeforeGenerating()
        {
            var oldBuilder = FromLines(Rules, SampleField);
            foreach (var position in builder.EnumeratePositions())
                builder[position].Should().Be(oldBuilder[position]);
        }

        [Test]
        public void ModifyBuilder_AfterGenerating()
        {
            var field = generator.Generate();
            var oldBuilder = FromLines(Rules, SampleField);
            foreach (var position in field.EnumeratePositions())
                if (oldBuilder[position])
                    builder[position].Should().BeTrue();
        }

        [Test]
        public void ModifyBuilderCorrectly_AfterGenerating()
        {
            var field = generator.Generate();
            foreach (var position in field.EnumeratePositions())
                builder[position].Should().Be(field[position] is IShipCell);
        }

        [Test]
        public void FailGenerating_WhenBuilderIsIncorrect()
        {
            var builder = A.Fake<IGameFieldBuilder>();
            A.CallTo(() => builder.ShipsLeft).Returns(new Dictionary<ShipType, int> {{ShipType.Battleship, -1}});
            Action generating = () => new RandomFieldGenerator(builder).Generate();
            generating.ShouldThrow<Exception>();
        }

        #region Builder generating

        private static readonly string[] SampleField =
        {
            ".X......X.",
            ".X........",
            "...XXXX...",
            "..........",
            "...XXX....",
            "..........",
            "..........",
            "..........",
            "..........",
            "........XX"
        };


        private static IGameFieldBuilder FromLines(GameRules rules, string[] lines)
        {
            var builder = new GameFieldBuilder(rules);
            for (var row = 0; row < rules.FieldSize.Height; row++)
                for (var column = 0; column < rules.FieldSize.Width; column++)
                    if (lines[row][column] == 'X')
                        builder.TryAddShipCell(new CellPosition(row, column));
            return builder;
        }

        #endregion
    }
}
