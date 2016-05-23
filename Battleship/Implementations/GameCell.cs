using Battleship.Interfaces;

namespace Battleship.Implementations
{
    public class GameCell : IGameCell
    {
        public CellType Type { get; }
        public bool Damaged { get; set; }

        public GameCell(CellType type)
        {
            Type = type;
        }

        protected bool Equals(GameCell other)
        {
            return 
                Type == other.Type &&
                Damaged == other.Damaged;
        }

        public override bool Equals(object obj)
        {
            var other = obj as GameCell;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return -1;
        }
    }
}
