using Newtonsoft.Json;
using System.Drawing;

namespace BattleShipsServer
{
    [Serializable]
    public class User
    {
        public string Name { get; set; }
        public bool YourTurn { get; set; }

        public Dictionary<string, Cell> Board = new Dictionary<string, Cell>();

        public bool isReady { get; set; }
        public bool HasFired { get; set; }
        public bool HasHit { get; set; }
    }
}