using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipsServer
{
    public class Initialize : IState
    {
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
            
        }

        public void Exit()
        {
            
        }
    }
}
