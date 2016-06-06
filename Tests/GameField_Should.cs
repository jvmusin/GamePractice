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

        #region Survived ships counter tests

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

        #region Contains method tests

        [Test]
        public void ReturnTrue_IfPositionIsInsideTheField()
        {
            var position = new CellPosition(4, 5);
            field.Contains(position).Should().BeTrue();
        }

        [Test]
        public void ReturnFalse_IfPositionIsOutsideTheField()
        {
            var position = new CellPosition(50, 9);
            field.Contains(position).Should().BeFalse();
        }

        [Test]
        public void ReturnTrue_IfPositionIsOnTheBorder()
        {
            var position = new CellPosition(9, 9);
            field.Contains(position).Should().BeTrue();
        }

        [Test]
        public void ReturnFalse_IfPositionIsAtTheOuterAngle()
        {
            var position = new CellPosition(10, 10);
            field.Contains(position).Should().BeFalse();
        }

        [Test]
        public void ReturnFalse_IfPositionHasNegativeCoordinates()
        {
            var position = new CellPosition(-1, 5);
            field.Contains(position).Should().BeFalse();
        }

        #endregion

        #region Indexer tests

        [Test]
        public void ReturnCorrectValuesByIndex()
        {
            foreach (var position in field.EnumeratePositions())
            {
                var cell = field[position];
                var symbolInField = SampleField[position.Row][position.Column];

                if (symbolInField == 'X') cell.Should().BeAssignableTo<IShipCell>();
                else cell.Should().BeAssignableTo<IEmptyCell>();

                cell.Damaged.Should().BeFalse();
            }
        }

        #endregion

        #region Shoot method tests

        [Test]
        public void Fail_IfTargetIsNull()
        {
            Action act = () => field.Shoot(null);
            act.ShouldThrow<Exception>();
        }

        [Test]
        public void ReturnNull_IfTargetIsOutsideTheField()
        {
            var target = new CellPosition(2, 13);
            field.Shoot(target).Should().BeNull();
        }

        [Test]
        public void ReturnShotResult_IfTargetIsInsideTheField()
        {
            var target = new CellPosition(4, 6);
            field.Shoot(target).Should().NotBeNull();
        }

        [Test]
        public void ReturnNull_IfTargetIsDamaged()
        {
            var target = new CellPosition(3, 3);
            field.Shoot(target);
            field.Shoot(target).Should().BeNull();
        }

        [Test]
        public void ReturnRightShotPosition()
        {
            var target = new CellPosition(3, 6);
            field.Shoot(target).Target.Should().Be(target);
        }

        [Test]
        public void ReturnDamagedCell_IfWeShotIt()
        {
            var target = new CellPosition(4, 4);
            field.Shoot(target);
            field[target].Damaged.Should().BeTrue();
        }

        [Test]
        public void ReturnCorrectShotType_WhenWeMissed()
        {
            var target = new CellPosition(0, 3);
            field.Shoot(target).Type.Should().Be(ShotType.Miss);
        }

        [Test]
        public void ReturnCorrectShotType_WhenWeHit()
        {
            var target = new CellPosition(0, 1);
            field.Shoot(target).Type.Should().Be(ShotType.Hit);
        }

        [Test]
        public void ReturnCorrectShotType_WhenWeKilled()
        {
            var target = new CellPosition(0, 4);
            field.Shoot(target).Type.Should().Be(ShotType.Kill);
        }

        [Test]
        public void ReturnEmptyAffectedCells_WhenWeMissed()
        {
            var target = new CellPosition(2, 1);
            field.Shoot(target).AffectedCells.Should().BeEmpty();
        }

        [Test]
        public void ReturnByAngleConnectedCells_WhenWeHit()
        {
            var target = new CellPosition(9, 8);
            var affectedCells = new[] {new CellPosition(8, 7), new CellPosition(8, 9)};

            field.Shoot(target).AffectedCells.Should().BeEquivalentTo(affectedCells);
        }

        [Test]
        public void ReturnCellsAround_WhenWeKilled()
        {
            var ship = new[]
            {
                new CellPosition(4, 3),
                new CellPosition(4, 4),
                new CellPosition(4, 5) 
            };

            var aroundCells = new List<CellPosition>();
            for (var row = 3; row <= 5; row += 2)
                for (var column = 2; column <= 6; column++)
                    aroundCells.Add(new CellPosition(row, column));
            aroundCells.Add(new CellPosition(4, 2));
            aroundCells.Add(new CellPosition(4, 6));

            var allAffectedCells = ship.SelectMany(pos => field.Shoot(pos).AffectedCells);
            allAffectedCells.Should().BeEquivalentTo(aroundCells);
        }

        [Test]
        public void MarkKilledShipAsKilled()
        {
            var ship = new[]
            {
                new CellPosition(4, 3),
                new CellPosition(4, 4),
                new CellPosition(4, 5)
            };
            foreach (var shipCell in ship)
                field.Shoot(shipCell);

            foreach (var shipCell in ship)
                ((IShipCell) field[shipCell]).Ship.Killed.Should().BeTrue();
        }

        #endregion

        #region Field building

        private static readonly string[] SampleField =
        {
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
