﻿// See https://aka.ms/new-console-template for more information
using BattleShipsServer;
using System.Drawing;

public class Messages
{
    public enum MessageType { movement, snapshot, join, initialJoin, chatmessage, chatUpdate, turnUpdate, checkConnection, sendBoard }
    public enum Direction { up, down }

   

    [Serializable]
    public class NetworkMessage
    {
        public MessageType type;
        public NetworkMessageBase message;
    }
    [Serializable]
    public class NetworkMessageBase
    {
        
    }
    [Serializable]
    public class PlayerMovementUpdate : NetworkMessageBase
    {
        public Direction direction;
    }

    [Serializable]
    public class SnapShot : NetworkMessageBase
    {
        public List<float> playerYPos;
        public int ballXpos;
        public int ballYpos;
    }

    [Serializable]
    public class JoinMessage : NetworkMessageBase
    {
        public string playerName;
        public int ResolutionX;
        public int ResolutionY;
    }

    [Serializable]
    public class ChatMessage : NetworkMessageBase
    {
        public string chatMessage;
        public string Name;
    }

    public class UpdateChat : NetworkMessageBase
    {
        public string LastMessage;
        public string Name;
    }

    [Serializable]
    public class TurnUpdate : NetworkMessageBase
    {

    }

    [Serializable]
    public class CheckConnection : NetworkMessageBase
    {
        public string Name;
    }

    [Serializable]
    public class SetInitialPositionsMessage : NetworkMessageBase
    {
        public int leftPlayerXPos;
        public int leftPlayerYPos;
        public int rightPlayerXPos;
        public int rightPlayerYPos;
        public int ballXPos;
        public int ballYPos;
    }
    [Serializable]
    public class SendBoard : NetworkMessageBase
    {
        public Dictionary<string, Cell> Board { get; set; }
        public string Name;
    }
}