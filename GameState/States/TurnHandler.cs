namespace BattleShipsServer
{
    public class TurnHandler : IState
    {
        private User currentUser;
        public User CurrentUser { get => currentUser; set => currentUser = value; }

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
            HandleTurn();
        }

        public void Exit()
        {
            
        }

        public void HandleTurn()
        {
            if (currentUser.HasFired)
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

            if (currentUser == GameStateController.Instance.User[1])
            {
                currentUser.HasFired = false;
                currentUser = GameStateController.Instance.User[0];
            }

        }
    }
}
