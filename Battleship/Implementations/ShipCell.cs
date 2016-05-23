namespace Battleship
{
    public class ShipCell : IGameCell
    {
        public bool Damaged { get; set; }

        protected bool Equals(ShipCell other)
        {
            return Damaged == other.Damaged;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ShipCell;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return -1;
        }
    }
}
