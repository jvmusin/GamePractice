using System;
using System.Collections.Generic;
using System.Drawing;
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
    public class GameController_Should
    {
        private static readonly GameRules Rules = GameRules.Default;
        private IPlayer player1;
        private IPlayer player2;
        private GameController gameController;

        [SetUp]
        public void SetUp()
        {
            player1 = A.Fake<IPlayer>();
            player2 = A.Fake<IPlayer>();

            A.CallTo(() => player1.SelfField.Rules).Returns(Rules);
            A.CallTo(() => player2.SelfField.Rules).Returns(Rules);

            gameController = new GameController(player1, player2);
        }

        #region Base tests

        [Test]
        public void SayThatGameNotFinished_WhenJustCreated()
        {
            gameController.GameFinished.Should().BeFalse();
        }

        [Test]
        public void ContainRightRules_WhenJustCreated()
        {
            gameController.Rules.Should().Be(Rules);
        }

        [Test]
        public void ContainRealPlayers_WhenJustCreated()
        {
            gameController.FirstPlayer.Should().Be(player1);
            gameController.SecondPlayer.Should().Be(player2);
        }

        [Test]
        public void ContainRealCurrentAndOpponentPlayers_WhenJustCreated()
        {
            gameController.CurrentPlayer.Should().Be(player1);
            gameController.OpponentPlayer.Should().Be(player2);
        }

        [Test]
        public void SwapActivePlayers_AfterMiss()
        {
            var target = new CellPosition(5, 6);
            A.CallTo(() => player2.SelfField.Shoot(target)).Returns(ShotResult.Miss(target));
            A.CallTo(() => player2.SelfField.SurvivedShips.Values).Returns(new[] {1});
            gameController.Shoot(target);

            gameController.CurrentPlayer.Should().Be(player2);
            gameController.OpponentPlayer.Should().Be(player1);
        }

        [Test]
        public void ContainSamePlayersAsFirstAndSecond_AfterMiss()
        {
            var target = new CellPosition(5, 6);
            A.CallTo(() => player2.SelfField.Shoot(target)).Returns(ShotResult.Miss(target));
            A.CallTo(() => player2.SelfField.SurvivedShips.Values).Returns(new[] {1});
            gameController.Shoot(target);

            gameController.FirstPlayer.Should().Be(player1);
            gameController.SecondPlayer.Should().Be(player2);
        }

        [Test]
        public void FailCreating_WhenRulesDiffer()
        {
            var rules1 = new GameRules(new Size(10, 5), new Dictionary<ShipType, int>());
            var rules2 = new GameRules(new Size(12, 1), new Dictionary<ShipType, int>());
            A.CallTo(() => player1.SelfField.Rules).Returns(rules1);
            A.CallTo(() => player2.SelfField.Rules).Returns(rules2);

            Action creating = () => new GameController(player1, player2);
            creating.ShouldThrow<Exception>();
        }

        #endregion

        #region Shoot method tests

        [Test]
        public void ReturnNull_WhenShotFailed()
        {
            var target = new CellPosition(5, 6);
            A.CallTo(() => player2.SelfField.Shoot(target)).Returns(null);

            gameController.Shoot(new CellPosition(5, 6)).Should().BeNull();
        }

        [Test]
        public void FinishGame_WhenOpponentKilled()
        {
            var target = new CellPosition(5, 6);
            A.CallTo(() => player2.SelfField.Shoot(target)).Returns(ShotResult.Miss(target));
            A.CallTo(() => player2.SelfField.SurvivedShips.Values).Returns(new[] {0});

            gameController.Shoot(target);
            gameController.GameFinished.Should().BeTrue();
        }

        [Test]
        public void NotResetGameFinishedState_WhenAskedTwice()
        {
            var target = new CellPosition(5, 6);
            A.CallTo(() => player2.SelfField.Shoot(target)).Returns(ShotResult.Miss(target));

            gameController.Shoot(target);
            gameController.GameFinished.Should().BeTrue();
            gameController.GameFinished.Should().BeTrue();
        }

        [Test]
        public void RememberAffectedCellsAndTarget_InOpponentKnowlendge_AfterHit()
        {
            var knowledge = new GameFieldKnowledge(gameController.Rules.FieldSize);
            A.CallTo(() => player1.OpponentFieldKnowledge).Returns(knowledge);

            var target = new CellPosition(5, 6);
            var affectedCells = new[] {new CellPosition(0, 0), new CellPosition(4, 4), new CellPosition(6, 6)};
            var shotResult = ShotResult.Hit(target, affectedCells);
            A.CallTo(() => player2.SelfField.Shoot(target)).Returns(shotResult);

            gameController.Shoot(target).Should().Be(shotResult);

            foreach (var position in knowledge.EnumeratePositions())
            {
                var expected = position.Equals(target)
                    ? true
                    : (affectedCells.Contains(position)
                        ? (bool?) false
                        : null);
                knowledge[position].Should().Be(expected);
            }
        }

        [Test]
        public void RememberTarget_AfterMiss()
        {
            var knowledge = new GameFieldKnowledge(gameController.Rules.FieldSize);
            A.CallTo(() => player1.OpponentFieldKnowledge).Returns(knowledge);
            
            var target = new CellPosition(5, 6);
            var shotResult = ShotResult.Miss(target);
            A.CallTo(() => player2.SelfField.Shoot(target)).Returns(shotResult);

            gameController.Shoot(target).Should().Be(shotResult);

            foreach (var position in knowledge.EnumeratePositions())
                knowledge[position].Should().Be(position.Equals(target) ? (bool?) false : null);
        }

        [Test]
        public void NotTryToAccessUnnecessaryFields_AfterHit()
        {
            A.CallTo(() => player2.OpponentFieldKnowledge).Returns(null);
            A.CallTo(() => player1.SelfField).Returns(null);

            gameController.Shoot(new CellPosition(5, 6));
        }

        [Test]
        public void ReallySwapActivePlayers_AfterMiss()
        {
            var target = new CellPosition(5, 6);

            A.CallTo(() => player2.SelfField.Shoot(target)).Returns(ShotResult.Miss(target));
            A.CallTo(() => player2.SelfField.SurvivedShips.Values).Returns(new[] {1});
            gameController.Shoot(target);

            var knowledge = new GameFieldKnowledge(gameController.Rules.FieldSize);
            A.CallTo(() => player2.OpponentFieldKnowledge).Returns(knowledge);
            var affectedCells = new[] {new CellPosition(2, 2), new CellPosition(1, 8)};
            A.CallTo(() => player1.SelfField.Shoot(target)).Returns(ShotResult.Hit(target, affectedCells));
            gameController.Shoot(target);
            
            foreach (var position in knowledge.EnumeratePositions())
            {
                var expected = position.Equals(target)
                    ? true
                    : (affectedCells.Contains(position)
                        ? (bool?)false
                        : null);
                knowledge[position].Should().Be(expected);
            }
        }

        #endregion
    }
}
