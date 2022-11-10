using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Text;

enum PacketType
{
    StartGame = 0,
    TurnInfo,
    RegisterTurn,
};

enum Status
{
    Ok = 0,
    Error,
}

public class Communication
{
    readonly string SERVER_ADDR = "localhost";
    readonly int SERVER_PORT = 7000;
    readonly TCPClient client;
    TurnState turnState;

    public Communication()
    {
        client = new TCPClient(SERVER_ADDR, SERVER_PORT)
        {
            OnReceive = ProcessPacketFromServer,
            OnReady = SendJoinLobby
        };
    }

    public void Init(TurnState turnState)
    {
        this.turnState = turnState; 
        turnState.OnNotResetChange(() =>
        {
            SendTurnInfo(turnState);
        });
    }

    public void Start()
    {
        client.Connect();
    }

    private void ProcessPacketFromServer(byte[] packet)
    {
        var packetAsStr = Encoding.ASCII.GetString(packet);
        var definition = new { Type = 0, Status = Status.Error };
        var partialJson = JsonConvert.DeserializeAnonymousType(packetAsStr, definition);
        var packetType = (PacketType)partialJson.Type;

        //Debug.Log(string.Format("Got packet {0}", packetType));

        var status = partialJson.Status;
        if (status != Status.Ok)
        {
            Debug.Log(string.Format("Packet {0} is not Ok", packetType));
            
            if (packetType == PacketType.RegisterTurn)
            {
                Debug.Log("Register turn error");
            }

            return;
        }

        // TODO: test token etc.

        switch (packetType)
        {
            case PacketType.StartGame: 
                ProcessStartGame(packetAsStr);
                break;
            case PacketType.TurnInfo:
                ProcessTurnInfo(packetAsStr);
                break;
        }
    }

   
    private void SendRequestPacket(PacketType packetType, string payload)
    {
        var sb = new StringBuilder();
        var sw = new StringWriter(sb);
        var jw = new JsonTextWriter(sw);

        var playerInfo = GameController.instance.PlayerInfo;

        jw.WriteStartObject();

        jw.WritePropertyName("ValidationHeader");
        jw.WriteValue("DEADBEE");

        jw.WritePropertyName("Type");
        jw.WriteValue(packetType);

        jw.WritePropertyName("Token");
        jw.WriteValue(playerInfo.Token);

        if (payload.Length > 0)
        {
            jw.WritePropertyName("Payload");
            jw.WriteRawValue(payload);
        }

        jw.WriteEndObject();

        var jsonAsBytes = Encoding.ASCII.GetBytes(sb.ToString());
        client.Send(jsonAsBytes);
    }

    private void SendTurnInfo(TurnState turnState)
    {
        var payload = new 
        {
            IsPlayerMoving = turnState.IsPlayerMoving,
            PlayerMoveDirection = turnState.PlayerMoveDirection,
        };

        SendRequestPacket(PacketType.RegisterTurn, JsonConvert.SerializeObject(payload));
    }

    private void ProcessStartGame(string packetAsStr)
    {
        ProcessTurnInfo(packetAsStr);
        GameController.instance.StartGame();
    }

    private void ProcessTurnInfo(string jsonStr)
    {
        var definition = new
        {
            Payload = new
            {
                IsExit = false,
                IsSpawn = false,
                Connections = new
                {
                    Top = false,
                    Right = false,
                    Bottom = false,
                    Left = false,
                }
            },
        };
        var turnInfoJson = JsonConvert.DeserializeAnonymousType(jsonStr, definition);

        var playerInfo = GameController.instance.PlayerInfo;
        playerInfo.PointInfo.IsExit = turnInfoJson.Payload.IsExit;
        playerInfo.PointInfo.IsSpawn = turnInfoJson.Payload.IsSpawn;
        playerInfo.PointInfo.HasTopConnection = turnInfoJson.Payload.Connections.Top;
        playerInfo.PointInfo.HasRightConnection = turnInfoJson.Payload.Connections.Right;
        playerInfo.PointInfo.HasBottomConnection = turnInfoJson.Payload.Connections.Bottom;
        playerInfo.PointInfo.HasLeftConnection = turnInfoJson.Payload.Connections.Left;

        turnState.Reset();
    }

    private void SendJoinLobby()
    {
        SendRequestPacket(PacketType.StartGame, "");
    }
}
