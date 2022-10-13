using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipsServer
{
    public class GameStateController
    {
        private IState currentGameState;
        public IState CurrentGameState { get => currentGameState; set => currentGameState = value; }

        private List<User> user = new List<User>();

        public List<User> User { get => user; set => user = value; }

        //Maybe used to keep track of board[1] board[2] for turns and such
        public Dictionary<string, Cell>[]Boards = new Dictionary<string, Cell>[2];


        private static GameStateController instance;
        public static GameStateController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameStateController();
                }
                return instance;
            }

        }

        

        public void ChangeGameState(IState nextGameState)
        {
            if (currentGameState != null)
            {
                currentGameState.Exit();
            }
            currentGameState = nextGameState;

            currentGameState.Enter();


        }

        public void UpdateGameState()
        {
            if (currentGameState != null)
            {
                currentGameState.Execute();
            }

        }
    }
}
