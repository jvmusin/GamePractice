using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class SmartPlayer : Player
    {
        private readonly Random rnd = new Random();

        private Size FieldSize => OpponentFieldKnowledge.Size;

        public SmartPlayer(IGameField selfField) : base(selfField)
        {
        }

        private IGameField prediction;

        public override CellPosition NextTarget
        {
            get
            {
//                if (!CanPredictionBeReal())
                    GenerateNewPrediction();
                if (!CanPredictionBeReal())
                    throw null;

                var damagedShip = FindDamagedShip().ToList();
                if (damagedShip.Any())
                {
                    var targets = damagedShip
                        .SelectMany(x => x.ByEdgeNeighbours)
                        .Where(x => OpponentFieldKnowledge.IsOnField(x))
                        .Where(x => prediction[x] is IShipCell && !OpponentFieldKnowledge[x].HasValue)
                        .ToList();
                    return targets[rnd.Next(targets.Count)];
                }
                else
                {
                    var targets = prediction.EnumeratePositions()
                        .Where(x => prediction[x] is IShipCell && !OpponentFieldKnowledge[x].HasValue)
                        .ToList();
                    return targets[rnd.Next(targets.Count)];
                }
            }
        }

        private void GenerateNewPrediction()
        {
            prediction = null;
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
                    if ((prediction = generator.Generate(x => OpponentFieldKnowledge[x] != false)) != null)
                        break;
                    builder.TryRemoveFullShip(variant.Item1, variant.Item2, variant.Item3);
                }
            }
            else prediction = generator.Generate(x => OpponentFieldKnowledge[x] != false);
        }

        private IEnumerable<Tuple<ShipType, CellPosition, bool>> GenerateContinuesForDamagedShip(
            IList<CellPosition> damagedShip, IGameFieldBuilder builder, bool vertical, ShipType ship)
        {
            if (builder.ShipsLeft[ship] == 0)
                yield break;

            var topLeftCell = GetTopLeftCell(damagedShip);
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

        private CellPosition GetTopLeftCell(IEnumerable<CellPosition> cells)
        {
            return cells.Min(cell => Tuple.Create(cell.Row * FieldSize.Width + cell.Column, cell)).Item2;
        }

        private IEnumerable<CellPosition> FindDamagedShip()
        {
            var damagedShip = OpponentFieldKnowledge.EnumeratePositions()
                .Where(position =>
                    OpponentFieldKnowledge[position] == true &&
                    position.AllNeighbours.Any(neighbour =>
                        OpponentFieldKnowledge.IsOnField(neighbour) &&
                        OpponentFieldKnowledge[neighbour] == null)).ToList();
            if (!damagedShip.Any())
                return damagedShip;
            return FindAllConnectedCells(damagedShip.First());
        }

        private IEnumerable<CellPosition> FindAllConnectedCells(CellPosition start)
        {
            var visited = new HashSet<CellPosition> { start };
            var queue = new Queue<CellPosition>();
            queue.Enqueue(start);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                var ways = current.ByEdgeNeighbours
                    .Where(x =>
                        OpponentFieldKnowledge.IsOnField(x) && !visited.Contains(x) &&
                        OpponentFieldKnowledge[x] == true);
                foreach (var connected in ways)
                {
                    visited.Add(connected);
                    queue.Enqueue(connected);
                }
            }

            return visited;
        }

        private bool CanPredictionBeReal()
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
