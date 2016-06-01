using System;
using System.Collections.Generic;
using System.Linq;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations
{
    public class SmartPlayer : Player
    {
        private readonly Random rnd = new Random();

        public SmartPlayer(IGameField selfField) : base(selfField)
        {
        }
        

        public override CellPosition NextTarget
        {
            get
            {
                var prediction = GenerateNewPrediction();
                if (!CanPredictionBeReal(prediction))
                    throw null;

                IOrderedEnumerable<CellPosition> targets;
                var damagedShip = FindDamagedShip().ToList();
                if (damagedShip.Any())
                {
                    targets = damagedShip.SelectMany(x => x.ByEdgeNeighbours)
                        .Where(x => OpponentFieldKnowledge.IsOnField(x))
                        .Where(x => prediction[x] is IShipCell)
                        .Where(x => !OpponentFieldKnowledge[x].HasValue)
                        .OrderBy(x => 0);
                }
                else
                {
                    targets = prediction.EnumeratePositions()
                        .Where(x => prediction[x] is IShipCell)
                        .Where(x => !OpponentFieldKnowledge[x].HasValue)
                        .OrderByDescending(x => ((IShipCell) prediction[x]).Ship.Length);
                }

                return targets
                    .ThenByDescending(x => x.ByVertexNeighbours
                        .Where(y => OpponentFieldKnowledge.IsOnField(y))
                        .Count(y => OpponentFieldKnowledge[y] == null))
                    .ThenBy(x => rnd.Next()).First();
            }
        }

        private IGameField GenerateNewPrediction()
        {
            var builder = new GameFieldBuilder();
            foreach (var position in OpponentFieldKnowledge.EnumeratePositions())
                if (OpponentFieldKnowledge[position] == true)
                    builder.TryAddShipCell(position);

            var generator = new RandomFieldGenerator(builder);
            var damagedShip = FindDamagedShip().ToList();
            if (damagedShip.Any())
            {
                foreach (var cell in damagedShip)
                    builder.TryRemoveShipCell(cell);
                
                var variants = new[] {4, 3, 2, 1}.SelectMany(x => new[]
                {
                    new {Ship = (ShipType) x, Vertical = true},
                    new {Ship = (ShipType) x, Vertical = false}
                }).SelectMany(x => GenerateContinuesForDamagedShip(damagedShip, builder, x.Vertical, x.Ship));

                foreach (var variant in variants)
                {
                    builder.TryAddFullShip(variant.Item1, variant.Item2, variant.Item3);
                    var prediction = generator.Generate(x => OpponentFieldKnowledge[x] != false);
                    if (prediction != null)
                        return prediction;
                    builder.TryRemoveFullShip(variant.Item1, variant.Item2, variant.Item3);
                }
            }
            return generator.Generate(x => OpponentFieldKnowledge[x] != false);
        }

        private IEnumerable<Tuple<ShipType, CellPosition, bool>> GenerateContinuesForDamagedShip(
            IList<CellPosition> damagedShip, IGameFieldBuilder builder, bool vertical, ShipType ship)
        {
            if (builder.ShipsLeft[ship] == 0)
                yield break;

            var topLeftCell = damagedShip.Min();
            var delta = vertical ? CellPosition.DeltaDown : CellPosition.DeltaRight;

            var start = vertical
                ? new CellPosition(0, topLeftCell.Column)
                : new CellPosition(topLeftCell.Row, 0);
            for (; builder.IsOnField(start); start += delta)
            {
                if (!builder.CanBeAddedSafely(ship, start, vertical, x => OpponentFieldKnowledge[x] != false))
                    continue;
                var newShipCells = Enumerable.Range(0, ship.GetLength()).Select(x => start + delta*x).ToList();
                if (damagedShip.Any(x => !newShipCells.Contains(x)))
                    continue;
                yield return Tuple.Create(ship, start, vertical);
            }
        }

        private IEnumerable<CellPosition> FindDamagedShip()
        {
            var damagedShip = OpponentFieldKnowledge.EnumeratePositions()
                .Where(position =>
                    OpponentFieldKnowledge[position] == true &&
                    position.AllNeighbours.Any(neighbour =>
                        OpponentFieldKnowledge.IsOnField(neighbour) &&
                        OpponentFieldKnowledge[neighbour] == null))
                .Take(1)
                .ToList();
            return damagedShip.Any()
                ? OpponentFieldKnowledge.FindAllConnectedByEdgeCells(damagedShip.First(), knowledge => knowledge == true)
                : damagedShip;
        }

        private bool CanPredictionBeReal(IGameField prediction)
        {
            if (prediction == null)
                return false;

            return (
                from position in OpponentFieldKnowledge.EnumeratePositions()
                let knowledge = OpponentFieldKnowledge[position]
                where knowledge.HasValue
                select prediction[position] is IShipCell == knowledge)
                .All(x => x);
        }
    }
}
