namespace Battleship
{
    public class Ship
    {
        public ShipType Type { get; }
        public int Length => Type.GetLength();
        public int Id { get; }

        public Ship(ShipType type, int id)
        {
            Type = type;
            Id = id;
        }

        protected bool Equals(Ship other)
        {
            return
                Type == other.Type &&
                Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Ship;
            return other != null &&  Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Id*10 + Type.GetLength();
            }
        }
    }
}
