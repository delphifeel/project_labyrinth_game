using UnityEngine;
using System;

enum PacketType
{
    StartGame = 0,
    TurnInfo,
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
    readonly PlayerInfo playerInfo;

    public Communication(PlayerInfo playerInfo)
    {
        this.playerInfo = playerInfo;

        client = new TCPClient(SERVER_ADDR, SERVER_PORT)
        {
            OnReceive = ProcessPacketFromServer,
            OnReady = SendJoinLobby
        };
    }

    public void Start()
    {
        client.Connect();
    }

    private void ProcessPacketFromServer(byte[] packet)
    {
        PacketType packetType = (PacketType) BitConverter.ToUInt32(packet, 8);
        Debug.Log(string.Format("Got packet {0}", packetType));

        Status status = (Status) BitConverter.ToUInt32(packet, 4);
        if (status != Status.Ok)
        {
            Debug.Log(string.Format("Packet {0} is not Ok", packetType));
            return;
        }
        
        // TODO: test token etc.

        uint payloadSize =  BitConverter.ToUInt32(packet, 44);
        byte[] payload = new byte[payloadSize];
        Array.Copy(packet, 48, payload, 0, payloadSize);

        switch (packetType)
        {
            case PacketType.StartGame: 
                ProcessStartGame(payload);
                break;
            default: 
                Debug.LogError("Unknown packet");
                return;
        }
    }

    private void ProcessStartGame(byte[] payload)
    {
        int pos = 0;
        playerInfo.PointInfo.IsExit = BitConverter.ToBoolean(payload, pos);
        pos += 4;

        playerInfo.PointInfo.IsSpawn = BitConverter.ToBoolean(payload, pos);
        pos += 4;

        playerInfo.PointInfo.HasTopConnection = BitConverter.ToBoolean(payload, pos);
        pos += 4;

        playerInfo.PointInfo.HasRightConnection = BitConverter.ToBoolean(payload, pos);
        pos += 4;

        playerInfo.PointInfo.HasBottomConnection = BitConverter.ToBoolean(payload, pos);
        pos += 4;

        playerInfo.PointInfo.HasLeftConnection = BitConverter.ToBoolean(payload, pos);
        pos += 4;

        Debug.Log(playerInfo.ToString());

        GameController.instance.StartGame();
    }

    private byte[] MakeRequestPacket(PacketType packetType, byte[] payload)
    {
        byte[] result = new byte[48 + payload.Length];
        int resultPos = 0;
        byte[] buffer;

        // validation header
        buffer = BitConverter.GetBytes((uint)0xDEADBEE);
        Array.Copy(buffer, 0, result, resultPos, buffer.Length);
        resultPos += 4;

        // type
        buffer = BitConverter.GetBytes((uint)packetType);
        Array.Copy(buffer, 0, result, resultPos, buffer.Length);
        resultPos += 4;

        // player token
        Array.Copy(playerInfo.Token, 0, result, resultPos, playerInfo.Token.Length);
        resultPos += playerInfo.Token.Length;

        // payload size
        buffer = BitConverter.GetBytes((uint)payload.Length);
        Array.Copy(buffer, 0, result, resultPos, buffer.Length);
        resultPos += 4;

        // payload
        Array.Copy(payload, 0, result, resultPos, payload.Length);
        resultPos += payload.Length;

        return result;
    }

    private void ProcessTurnInfo(byte[] payload)
    {
    }

    private void SendJoinLobby()
    {
        byte[] buffer = MakeRequestPacket(PacketType.StartGame, new byte[0]);
        client.Send(buffer);
    }
}
