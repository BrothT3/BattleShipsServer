using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum TURNORDER { user1, user2 }
namespace BattleShipsServer.GameState.States
{
    public class TurnHandling : IState
    {
        private TurnHandling instance;
        public TurnHandling Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TurnHandling();
                }
                return instance;
            }
        }
        private TURNORDER turnOrder;
        public string User1key { get; set; }
        public string User2key { get; set; }
        public void Enter()
        {
            
          User1key = GameStateController.Instance.Boards[0].Keys.ToString();
          User2key = GameStateController.Instance.Boards[1].Keys.ToString();
            
        }

        public void Execute()
        {

        }

        public void Exit()
        {

        }
        public void TurnDelegation()
        {
            switch (turnOrder)
            {
                case TURNORDER.user1:

                    break;
                case TURNORDER.user2:

                    break;
            }
        }
    }
}
