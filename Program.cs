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



int port = 11000;

UdpClient listener = new UdpClient(port);
IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);

float updateInterval = 60;
System.Timers.Timer timer = new System.Timers.Timer();
timer.Interval = (double)1000f / updateInterval;

//game state, needs to be configured
int ballXPos = 0;
int ballYPos = 0;

int resX;
int resY;


timer.Elapsed += SendingTimer;

Thread listeningThread = new Thread(Listening);
listeningThread.Start();



void Listening()
{
    try
    {
        Console.WriteLine($"Listening on port: {port}");
        while (true)
        {
            Console.WriteLine("Waiting for data..");

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

    //update ball pos


    //is ball outside of resolution?? Does somehting happen?

    //Get player pos from worldstate?

    //All the actual game logic goes here. Or at least this is the starting point.


    SnapShot snapshot = new SnapShot() { ballXpos = ballXPos, ballYpos = ballYPos };
    SendTypedNetworkMessage(listener, groupEP, snapshot, MessageType.snapshot);
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
                //  SendTypedNetworkMessage(listener, groupEP, chatMessage, MessageType.chatmessage);
                ContactService(chatMessage);
                //  ContactService(chatMessage.Message);
                break;
            default:
                break;
        }
    }
}

void HandleChatMessage(IPEndPoint messageSenderInfo, UdpClient listener, ChatMessage receivedChatMessage)
{
    //Console.WriteLine(receivedChatMessage.chatMessage);

}

void HandleJoinMessage(IPEndPoint messageSenderInfo, UdpClient listener, JoinMessage recievedJoinMessage)
{
    ballXPos = recievedJoinMessage.ResolutionX / 2;
    ballYPos = recievedJoinMessage.ResolutionY / 2;
    resX = recievedJoinMessage.ResolutionX;
    resY = recievedJoinMessage.ResolutionY;
    //var networkMessage = new SetInitialPositionsMessage()
    //{

    //    ballXPos = recievedJoinMessage.ResolutionX / 2,
    //    ballYPos = recievedJoinMessage.ResolutionY / 2,
    //    leftPlayerXPos = 0,
    //    leftPlayerYPos = recievedJoinMessage.ResolutionY / 2,
    //    rightPlayerXPos = recievedJoinMessage.ResolutionX,
    //    rightPlayerYPos = recievedJoinMessage.ResolutionY / 2
    //};

    var networkMessage = new ChatMessage()
    {
        chatMessage = "tester"
    };


    SendTypedNetworkMessage(listener, messageSenderInfo, networkMessage, MessageType.join);
    //should actually start when two players have joined...
    timer.Start();
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

    Debug.WriteLine($"Sending json message{serializedNetworkMessage} to client..");
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

async void ContactService(ChatMessage message)
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

    GetChatMessage();
}

async void GetChatMessage()
{
    List<Chat> msg = null;
    HttpClient client = new HttpClient();
    string url = "https://localhost:7060/api/chat";

    var chatMsg = new Chat();
    var res = await client.GetAsync(url);


    //  string responseBody = await res.Content.ReadAsStringAsync();
    string responseBody = await res.Content.ReadAsStringAsync();

    chatMsg = JsonConvert.DeserializeObject<Chat>(responseBody);
    Console.WriteLine(chatMsg);









}
