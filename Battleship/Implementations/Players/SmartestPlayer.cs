using System.Linq;
using Battleship.Base;
using Battleship.Interfaces;
using Battleship.Utilities;

namespace Battleship.Implementations.Players
{
    public class SmartestPlayer : SmartPlayer
    {
        public SmartestPlayer(IGameField selfField) : base(selfField)
        {
        }

        public override CellPosition NextTarget
        {
            get
            {
                var predictionsCounter = new int[OpponentFieldKnowledge.Size.Height, OpponentFieldKnowledge.Size.Width];

                var predictions = Enumerable.Range(0, 100).Select(x => GenerateNewPrediction());
                foreach (var prediction in predictions)
                    foreach (var target in prediction.EnumeratePositions())
                        if (prediction[target] is IShipCell)
                            predictionsCounter[target.Row, target.Column]++;

                var damagedShip = FindDamagedShip().ToList();

                return predictionsCounter.EnumeratePositions()
                    .OrderByDescending(x => predictionsCounter.GetValue(x))
                    .First(x => !OpponentFieldKnowledge[x].HasValue &&
                                (!damagedShip.Any() || damagedShip.Any(y => y.ByEdgeNeighbours.Contains(x))));
            }
        }
    }
}
