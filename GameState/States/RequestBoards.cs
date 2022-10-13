using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (user == GameStateController.Instance.User[0])
            {
                user.YourTurn = true;
            }
            boards++;
            if (boards == 1)
            {
                GameStateController.Instance.Boards[0] = user.Board;
               

            }
            else if (boards == 2)
            {
                GameStateController.Instance.Boards[1] = user.Board;

                TurnHandler.Instance.Users[0] = GameStateController.Instance.User[0];
                TurnHandler.Instance.Users[1] = GameStateController.Instance.User[1];


            }
        }
        public void CheckBoards()
        {
            if (boards == 2)
            {
                GameStateController.Instance.ChangeGameState(TurnHandler.Instance);                           
            }
        }

    }
}
