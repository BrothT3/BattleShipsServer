using System.Drawing;
using static Messages;

namespace BattleShipsServer
{
    public class TurnHandler : IState
    {
        private User currentUser;
        public User CurrentUser { get => currentUser; set => currentUser = value; }

        public User[]? Users = new User[2];

        private static TurnHandler instance;
        public static TurnHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TurnHandler();
                }
                return instance;
            }

        }
        public void Enter()
        {
            currentUser = GameStateController.Instance.User[0];
        }

        public void Execute()
        {
            //HandleTurn();
        }

        public void Exit()
        {
            
        }

        public void HandleTurn(SendShotAttempt shot)
        {
            string point = TranslateStringToPoint(shot.MousePos);
            if (shot.Name == Users[0].Name)
            {
                currentUser = Users[0];
                if (Users[1].Board["{"+point+"}"].IsOccupied)
                {
                    Users[0].HasHit = true;
                    currentUser.HasHit = true;
                 
                }
                else
                {
                    Users[0].HasHit = false;
                    currentUser.HasHit = false;
                    currentUser.HasFired = true;
                    Users[0].HasFired = true;
                }
               
               
            }
            if (shot.Name == Users[1].Name)
            {
                currentUser = Users[1];
                if (Users[0].Board["{"+point+"}"].IsOccupied)
                {
                    Users[1].HasHit = true;
                    currentUser.HasHit = true;
                    

                }
                else
                {
                    Users[1].HasHit = false;
                    currentUser.HasHit = false;
                    currentUser.HasFired = true;
                    Users[1].HasFired = true;
                }
                

            }
            if (Users[0].HasFired)
            {
                ChangeUser(ref currentUser);
            }
            else if (Users[1].HasFired)
            {
                ChangeUser(ref currentUser);
            }

        }
        public void ChangeUser(ref User currentUser)
        {
            if (currentUser == GameStateController.Instance.User[0])
            {
                currentUser.HasFired = false;
                currentUser = GameStateController.Instance.User[1];
            }
            else if (currentUser == GameStateController.Instance.User[1])
            {
                currentUser.HasFired = false;
                currentUser = GameStateController.Instance.User[0];
            }

        }

        public string TranslateStringToPoint(string target)
        {

            string[] split = target.Split(' ');
            string tmpX = string.Empty;
            string tmpY = string.Empty;

            for (int i = 0; i < split[0].Length; i++)
            {
                if (char.IsDigit(split[0][i]))
                {
                    tmpX += split[0][i];
                }
                if (char.IsDigit(split[1][i]))
                {
                    tmpY += split[1][i];
                }
            }
            int x = int.Parse(tmpX);
            int y = int.Parse(tmpY) - 10;

            string result = $"X:{x} Y:{y}";
           
            return result;
        }
    }
}
