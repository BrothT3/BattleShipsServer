using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Messages;

namespace BattleShipsServer
{
    public class RequestBoards : IState
    {
        int boards = 0;
        private static RequestBoards instance;
        public static RequestBoards Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RequestBoards();
                }
                return instance;
            }

        }
        public void Enter()
        {

        }

        public void Execute()
        {
            CheckBoards();
        }

        public void Exit()
        {
            boards = 0;
        }
        public void GetBoard(SendBoard Board)
        {
            User user = GameStateController.Instance.User.Find(x => x.Name == Board.Name);
            user.Board = Board.Board;
            boards++;
        }
        public void CheckBoards()
        {
            if (boards == 2)
            {
                //GameStateController.Instance.ChangeGameState(trin3.instance);
            }
        }

    }
}
