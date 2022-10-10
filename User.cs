using System.Drawing;

namespace BattleShipsServer
{
    public class User
    {
        public string Name { get; set; }
        public bool YourTurn { get; set; }

        public Dictionary<Point, Cell> Board { get; set; }

        public bool isReady { get; set; }
    }
}