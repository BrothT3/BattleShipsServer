// See https://aka.ms/new-console-template for more information


using static Messages;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Numerics;
using System.Text;
using System.Timers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft;
using static System.Net.WebRequestMethods;
using BattleShipsServer;

int port = 11000;

UdpClient listener = new UdpClient(port);
IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);

SendMousePos opponentInfo = new SendMousePos();
float updateInterval = 60;
System.Timers.Timer timer = new System.Timers.Timer();
timer.Interval = (double)1000f / updateInterval;



//game state, needs to be configured
int ballXPos = 0;
int ballYPos = 0;

int resX;
int resY;

List<User> users = new List<User>();
//used to check if user is connected
string prevName = String.Empty;
List<string> connectedUsers = new List<string>();

UpdateChat chat = null;

timer.Elapsed += SendingTimer;

Thread listeningThread = new Thread(Listening);
listeningThread.Start();



void Listening()
{
    try
    {
        Console.WriteLine($"Server is online");
        while (true)
        {
            //  Console.WriteLine("Waiting for data..");

            var data = listener.Receive(ref groupEP);

            OtherHandleMessage(data, groupEP);
        }
    }
    catch (SocketException e)
    {


        Console.WriteLine(e);
    }
    finally
    {
        listener.Close();
    }
}

void SendingTimer(object? sender, ElapsedEventArgs e)
{
    //simular to update loop :)


    GameStateController.Instance.UpdateGameState();

    //is ball outside of resolution?? Does somehting happen?

    //Get player pos from worldstate?

    //All the actual game logic goes here. Or at least this is the starting point.





    //  SnapShot snapshot = new SnapShot() { ballXpos = ballXPos, ballYpos = ballYPos };
    //  SendTypedNetworkMessage(listener, groupEP, snapshot, MessageType.snapshot);
}


void OtherHandleMessage(byte[] data, IPEndPoint messageSenderInfo)
{
    var dataDeEncodeShouldbeJson = Encoding.UTF8.GetString(data);

    JObject? complexMessage = JObject.Parse(dataDeEncodeShouldbeJson);
    JToken? complexMessageType = complexMessage["type"];
    if (complexMessage != null && complexMessageType?.Type is JTokenType.Integer)
    {
        MessageType mesType = (MessageType)complexMessageType.Value<int>();
        JToken? complexMessageMessage = complexMessage["message"];
        if (complexMessageMessage == null)
        {
            return;
        }

        switch (mesType)
        {
            case MessageType.movement:
                PlayerMovementUpdate receivedMovement = complexMessage["message"].ToObject<PlayerMovementUpdate>();
                break;
            case MessageType.join:
                JoinMessage recievedJoinedMessage = complexMessage["message"].ToObject<JoinMessage>();
                HandleJoinMessage(messageSenderInfo, listener, recievedJoinedMessage);
                break;
            case MessageType.chatmessage:
                ChatMessage chatMessage = complexMessage["message"].ToObject<ChatMessage>();
                PostToService(chatMessage);
                break;
            case MessageType.chatUpdate:
                UpdateChat chatUpdate = complexMessage["message"].ToObject<UpdateChat>();
                GetChatMessage();
                break;
            case MessageType.checkConnection:
                CheckConnection checkConnection = complexMessage["message"].ToObject<CheckConnection>();
                ConnectionCheck(checkConnection);
                break;
            case MessageType.sendBoard:
                SendBoard sendBoard = complexMessage["message"].ToObject<SendBoard>();
                RequestBoards.Instance.GetBoard(sendBoard);
                break;
            case MessageType.sendMouseInfo:
                SendMousePos sendMousePos = complexMessage["message"].ToObject<SendMousePos>();
                SendMouseInfo(sendMousePos);
                break;
            case MessageType.receiveOpponentMouse:
                SendMousePos receiveMousePos = complexMessage["message"].ToObject<SendMousePos>();
                ReceiveOpponentMousePos(listener, groupEP);
                break;
            case MessageType.turnUpdate:
                TurnUpdate turnUpdate = complexMessage["message"].ToObject<TurnUpdate>();
                HandleTurns(listener, groupEP, turnUpdate);
                break;

            case MessageType.shoot:
                SendShotAttempt shot = complexMessage["message"].ToObject<SendShotAttempt>();
                TurnHandler.Instance.HandleTurn(shot);
                break;
            default:
                break;
        }
    }
}

async void HandleChatMessage(IPEndPoint groupEP, UdpClient listener)
{
    HttpClient client = new HttpClient();
    string url = "https://localhost:7060/api/chat";

    var chatMsg = new Chat();
    var res = await client.GetAsync(url);


    //string will be in json format
    string responseBody = await res.Content.ReadAsStringAsync();

    //desiralize to make it into a .NET object
    chatMsg = JsonConvert.DeserializeObject<Chat>(responseBody);

    var chatUpdate = new UpdateChat() { Name = chatMsg.Name, LastMessage = chatMsg.Message };

    SendTypedNetworkMessage(listener, groupEP, chatUpdate, MessageType.chatUpdate);



}

void HandleJoinMessage(IPEndPoint messageSenderInfo, UdpClient listener, JoinMessage recievedJoinMessage)
{

    if (users.Find(x => x.Name == recievedJoinMessage.playerName) == null)
    {
        users.Add(new User() { Name = recievedJoinMessage.playerName, YourTurn = false });
        GameStateController.Instance.User.Add(new User() { Name = recievedJoinMessage.playerName, YourTurn = false });
    }
    resX = recievedJoinMessage.ResolutionX;
    resY = recievedJoinMessage.ResolutionY;

    var networkMessage = new ChatMessage()
    {
        chatMessage = $"{recievedJoinMessage.playerName} has joined the game..",
        Name = "Server"
    };
    PostToService(networkMessage);


    timer.Start();
    //Initialize.Instance.users++;
    //when playercount is up and good shit's happening
    GameStateController.Instance.ChangeGameState(Initialize.Instance);

    //was supposed to change once 2 players connected, but we couldn't find a way to do this from inside our state pattern
    ChangeGameState change = new ChangeGameState() { nextGameState = GameState.placeShips };
    SendTypedNetworkMessage(listener, groupEP, change, MessageType.changeState);
}

static void SendTypedNetworkMessage(UdpClient listener, IPEndPoint groupEP, NetworkMessageBase networkMessageBase, MessageType messageType)
{
    var message = new NetworkMessage()
    {
        type = messageType,
        message = networkMessageBase

    };

    var serializedNetworkMessage = JsonConvert.SerializeObject(message);

    byte[] jsonAsBytes = Encoding.UTF8.GetBytes(serializedNetworkMessage);

    try
    {
        listener.Send(jsonAsBytes, groupEP);
    }
    catch (ObjectDisposedException)
    {
        Debug.WriteLine($"Connection lost..");

    }
    catch (SocketException)
    {
    }




}

async void PostToService(ChatMessage message)
{
    HttpClient client = new HttpClient();
    string url = "https://localhost:7060/api/chat";

    try
    {
        var chat = new Chat() { Name = message.Name, Message = message.chatMessage };
        var data = new StringContent(JsonConvert.SerializeObject(chat), Encoding.UTF8, "application/json");
        var res = await client.PostAsync(url + "/message", data);

    }
    catch (Exception)
    {


    }

}

async void GetChatMessage()
{
    try
    {
        HttpClient client = new HttpClient();
        string url = "https://localhost:7060/api/chat";

        var chatMsg = new Chat();
        var res = await client.GetAsync(url);

        //string will be in json format
        string responseBody = await res.Content.ReadAsStringAsync();

        //desiralize to make it into a .NET object
        chatMsg = JsonConvert.DeserializeObject<Chat>(responseBody);
        chat = new UpdateChat() { Name = chatMsg.Name, LastMessage = chatMsg.Message };

        SendTypedNetworkMessage(listener, groupEP, chat, MessageType.chatUpdate);


    }
    catch (Exception)
    {


    }



}


void ConnectionCheck(CheckConnection connection)
{


    try
    {
        string name = connection.Name;
        connectedUsers.Add(name);

        if (connectedUsers.Count >= 10)
        {
            string userOne = string.Empty;
            string userTwo = string.Empty;
            foreach (string userName in connectedUsers)
            {
                if (userOne == string.Empty)
                {
                    userOne = userName;

                }
                else if (userTwo == string.Empty && userName != userOne)
                {
                    userTwo = userName;
                }
            }

            while (users.Find(x => x.Name != userOne && x.Name != userTwo) != null)
            {
                User? tmp = users.Find(x => x.Name != userOne && x.Name != userTwo);
                users.Remove(tmp);
            }
            connectedUsers.Clear();
        }


    }
    catch (Exception)
    {


    }


}

void SendMouseInfo(SendMousePos sendMousePos)
{
    opponentInfo = new SendMousePos() { mousePos = sendMousePos.mousePos, Name = sendMousePos.Name };

    // SendTypedNetworkMessage(listener, groupEP, networkMessage, MessageType.receiveOpponentMouse);
}

void ReceiveOpponentMousePos(UdpClient listener, IPEndPoint groupEP)
{
    //var turnUpdate = new TurnUpdate()
    //{
    //    Name = GameStateController.Instance.User[0].Name,
    //    YourTurn = true
    //};
   
    //SendTypedNetworkMessage(listener, groupEP, turnUpdate, MessageType.turnUpdate);

    if (opponentInfo != null)
    {
        var networkMessage = new SendMousePos()
        {
            mousePos = opponentInfo.mousePos,
            Name = opponentInfo.Name
        };
        SendTypedNetworkMessage(listener, groupEP, networkMessage, MessageType.receiveOpponentMouse);


    }

}


void HandleTurns(UdpClient listener, IPEndPoint groupEP, TurnUpdate turnUpdate)
{
    if (GameStateController.Instance.CurrentGameState == TurnHandler.Instance && TurnHandler.Instance.CurrentUser != null)
    {

        var networkMessage = new TurnUpdate() { Name = TurnHandler.Instance.CurrentUser.Name,
            YourTurn = TurnHandler.Instance.CurrentUser.YourTurn , HasHit = TurnHandler.Instance.CurrentUser.HasHit};

        SendTypedNetworkMessage(listener, groupEP, networkMessage, MessageType.turnUpdate);
    }
}