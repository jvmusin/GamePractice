using Battleship.Base;
using Battleship.Implementations.GameCells;
using Battleship.Interfaces;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GameCell_Should
    {
        private CellPosition position;
        private EmptyCell cell;

        [SetUp]
        public void SetUp()
        {
            position = new CellPosition(5,7);
            cell = new EmptyCell(position);
        }

        [Test]
        public void ContainCorrectPosition()
        {
            cell.Position.Should().Be(position);
        }

        [Test]
        public void ContainCorrectShip()
        {
            var ship = A.Fake<IShip>();
            var cell = new ShipCell(position, ship);
            cell.Ship.Should().Be(ship);
        }

        [Test]
        public void BeUndamaged_WhenJustCreated()
        {
            cell.Damaged.Should().BeFalse();
        }

        [Test]
        public void BeAbleToBecomeDamaged()
        {
            cell.Damaged = true;
            cell.Damaged.Should().BeTrue();
        }

        [Test]
        public void BeEqualToSameCell()
        {
            var first = new EmptyCell(position);
            var second = new EmptyCell(position);
            var third = new EmptyCell(position + CellPosition.DeltaDown);
            first.Equals(second).Should().BeTrue();
            first.Equals(third).Should().BeFalse();
        }

        [Test]
        public void BeNotEqualToOtherCell()
        {
            var first = new EmptyCell(position);
            var second = new EmptyCell(position + CellPosition.DeltaDown);
            first.Equals(second).Should().BeFalse();
        }

        [Test]
        public void HaveSameHashWithSameCell()
        {
            var first = new EmptyCell(position);
            var second = new EmptyCell(position);

            var firstHash = first.GetHashCode();
            var secondHash = second.GetHashCode();

            firstHash.Should().Be(secondHash);
        }

        [Test]
        public void HaveDifferentHashWithDifferentCell()
        {
            var first = new EmptyCell(position);
            var second = new EmptyCell(position + CellPosition.DeltaDown);

            var firstHash = first.GetHashCode();
            var secondHash = second.GetHashCode();

            firstHash.Should().NotBe(secondHash);
        }
    }
}
