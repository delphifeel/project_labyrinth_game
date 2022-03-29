using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public enum MoveDirection
{
    Top = 1,
    Right,
    Bottom,
    Left,
}

public class CommandsProcessor
{
    private PlayerInfo playerInfo;
    private TCPClient client;
    private Action<List<MoveDirection>> onMove;

    public Action<PlayerInfo> OnPlayerInit = null;

    public CommandsProcessor(PlayerInfo playerInfo, Config config)
    {
        this.playerInfo = playerInfo;

        client = new TCPClient(config.gameServerAddr, (int)config.gameServerPort);
        client.OnReceive = OnReceiveCommand;
        client.OnReady = SendInit;
    }

    public void Start()
    {
        client.Connect();
    }
    public void SendMove(List<MoveDirection> directions, Action<List<MoveDirection>> onMove)
    {
        this.onMove = onMove;
        byte[] payload = new byte[8];
        byte[] buffer = BitConverter.GetBytes((uint)directions[0]);
        Array.Copy(buffer, 0, payload, 0, buffer.Length);
        if (directions.Count > 1)
        {
            buffer = BitConverter.GetBytes((uint)directions[1]);
            Array.Copy(buffer, 0, payload, 4, buffer.Length);
        }
        
        client.Send(MakeRequestCommand(CommandType.PlayerMove, payload));
    }

    private void OnReceiveCommand(byte[] command)
    {
        CommandType commandType = (CommandType) BitConverter.ToUInt32(command, 4);

        // extract payload
        int payloadSize = command.Length - 8;
        byte[] payload = new byte[payloadSize];
        Array.Copy(command, 8, payload, 0, payloadSize);

        // parse by command type
        if (commandType == CommandType.PlayerInit)
        {
            ProcessPlayerInit(payload);
            return;
        } else if (commandType == CommandType.PlayerMove)
        {
            ProcessPlayerMove(payload);
            return;
        }

        Debug.Log("Unknown command");
    }

    private void SendInit()
    {
        client.Send(MakeRequestCommand(CommandType.PlayerInit, new byte[4]));
    }

    private byte[] MakeRequestCommand(CommandType commandType, byte[] payload)
    {
        byte[] result = new byte[48 + payload.Length];
        int resultPos = 0;
        byte[] buffer;

        // validation header
        buffer = BitConverter.GetBytes((uint)0xDEADBEAF);
        Array.Copy(buffer, 0, result, resultPos, buffer.Length);
        resultPos += 4;

        // command type
        buffer = BitConverter.GetBytes((uint)commandType);
        Array.Copy(buffer, 0, result, resultPos, buffer.Length);
        resultPos += 4;

        // session index
        buffer = BitConverter.GetBytes((uint)playerInfo.SessionIndex);
        Array.Copy(buffer, 0, result, resultPos, buffer.Length);
        resultPos += 4;

        // player index
        buffer = BitConverter.GetBytes((uint)playerInfo.PlayerIndex);
        Array.Copy(buffer, 0, result, resultPos, buffer.Length);
        resultPos += 4;

        // player token
        Array.Copy(playerInfo.Token, 0, result, resultPos, playerInfo.Token.Length);
        resultPos += playerInfo.Token.Length;

        // payload
        Array.Copy(payload, 0, result, resultPos, payload.Length);
        resultPos += payload.Length;

        return result;
    }

    private void ProcessPlayerInit(byte[] payload)
    {
        playerInfo.X = BitConverter.ToInt32(payload, 0);
        playerInfo.Y = BitConverter.ToInt32(payload, 4);
        playerInfo.LabPoint.Id = BitConverter.ToUInt32(payload, 8);
        playerInfo.LabPoint.TopConnectionId = BitConverter.ToUInt32(payload, 12);
        playerInfo.LabPoint.RightConnectionId = BitConverter.ToUInt32(payload, 16);
        playerInfo.LabPoint.BottomConnectionId = BitConverter.ToUInt32(payload, 20);
        playerInfo.LabPoint.LeftConnectionId = BitConverter.ToUInt32(payload, 24);
        playerInfo.LabPoint.IsExit = BitConverter.ToBoolean(payload, 28);
        playerInfo.LabPoint.IsSpawn = BitConverter.ToBoolean(payload, 32);

        if (OnPlayerInit != null)
        {
            OnPlayerInit(playerInfo);
        }
    }

    private void ProcessPlayerMove(byte[] payload)
    {
        if (BitConverter.ToBoolean(payload, 0) == false)
        {
            return;
        }
        List<MoveDirection> directions = new List<MoveDirection>();
        directions.Add((MoveDirection) BitConverter.ToUInt32(payload, 4));
        uint secondDirection = BitConverter.ToUInt32(payload, 8);
        if (secondDirection != 0)
        {
            directions.Add((MoveDirection)secondDirection);
        }

        this.onMove(directions);
    }
}
