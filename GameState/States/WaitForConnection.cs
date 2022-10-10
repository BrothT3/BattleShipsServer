using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipsServer
{
    public class Initialize : IState
    {
        public int users = 0;
        private static Initialize instance;
        public static Initialize Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Initialize();
                }
                return instance;
            }

        }
        public void Enter()
        {
            
        }

        public void Execute()
        {
            WaitForUsers();
        }

        public void Exit()
        {
            
        }

        public void WaitForUsers()
        {
            if (users >= 2)
            {
                GameStateController.Instance.ChangeGameState(RequestBoards.Instance);
            }
        }
    }
}
