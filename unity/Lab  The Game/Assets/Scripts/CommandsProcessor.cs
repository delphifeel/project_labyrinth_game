using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class CommandsProcessor
{
    private enum Direction
    {
        Top = 1,
        Right,
        Bottom,
        Left,
    };

    private TCPClient client;

    public Action<bool, Vector2, Vector2> OnMove = null;
    public Action OnPlayerInit = null;

    public CommandsProcessor(Config config)
    {
        client = new TCPClient(config.gameServerAddr, (int)config.gameServerPort);
        client.OnReceive = OnReceiveCommand;
        client.OnReady = SendInit;
    }

    public void Start()
    {
        client.Connect();
    }

    private List<Direction> VectorDirectionToList(Vector2 directions)
    {
        List<Direction> result = new List<Direction>();
        if (directions.x == 1)
        {
            result.Add(Direction.Right);
        } else if (directions.x == -1)
        {
            result.Add(Direction.Left);
        }
        
        if (directions.y == 1)
        {
            result.Add(Direction.Top);
        } else if (directions.y == -1)
        {
            result.Add(Direction.Bottom);
        }

        return result;
    }

    private Vector2 ListDirectionToVector(List<Direction> l)
    {
        Vector2 result = new Vector2(0, 0);
        foreach (Direction d in l)
        {
            switch (d)
            {
                case Direction.Left: result.x = -1; break;
                case Direction.Right: result.x = 1; break;
                case Direction.Top: result.y = 1; break;
                case Direction.Bottom: result.y = -1; break;
            }
        }
        return result;
    }

    public void SendMove(Vector2 directions)
    {
        var directionsList = VectorDirectionToList(directions);
        byte[] payload = new byte[8];
        byte[] buffer = BitConverter.GetBytes((uint)directionsList[0]);
        Array.Copy(buffer, 0, payload, 0, buffer.Length);
        if (directionsList.Count > 1)
        {
            buffer = BitConverter.GetBytes((uint)directionsList[1]);
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
        var dummy = new byte[4];
        client.Send(MakeRequestCommand(CommandType.PlayerInit, dummy));
    }

    private byte[] MakeRequestCommand(CommandType commandType, byte[] payload)
    {
        byte[] result = new byte[52 + payload.Length];
        int resultPos = 0;
        byte[] buffer;
        PlayerInfo playerInfo = GameController.instance.playerInfo;

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

        // payload size
        buffer = BitConverter.GetBytes((uint)payload.Length);
        Array.Copy(buffer, 0, result, resultPos, buffer.Length);
        resultPos += 4;

        // payload
        Array.Copy(payload, 0, result, resultPos, payload.Length);
        resultPos += payload.Length;

        return result;
    }

    private void ProcessPlayerInit(byte[] payload)
    {
        PlayerInfo playerInfo = GameController.instance.playerInfo;
        playerInfo.X = BitConverter.ToSingle(payload, 0);
        playerInfo.Y = BitConverter.ToSingle(payload, 4);
        playerInfo.Speed = BitConverter.ToSingle(payload, 8);
        playerInfo.PointInfo.HasTopConnection = BitConverter.ToBoolean(payload, 12);
        playerInfo.PointInfo.HasRightConnection = BitConverter.ToBoolean(payload, 16);
        playerInfo.PointInfo.HasBottomConnection = BitConverter.ToBoolean(payload, 20);
        playerInfo.PointInfo.HasLeftConnection = BitConverter.ToBoolean(payload, 24);
        playerInfo.PointInfo.IsExit = BitConverter.ToBoolean(payload, 28);
        playerInfo.PointInfo.IsSpawn = BitConverter.ToBoolean(payload, 32);
        GameController.instance.roomSize = BitConverter.ToUInt32(payload, 36);

        if (OnPlayerInit != null)
        {
            OnPlayerInit();
        }
    }

    private void ProcessPlayerMove(byte[] payload)
    {
        // is success ?
        bool success = BitConverter.ToBoolean(payload, 0);
        List<Direction> directions = new List<Direction>();
        if (success)
        {
            directions.Add((Direction)BitConverter.ToUInt32(payload, 4));
            uint secondDirection = BitConverter.ToUInt32(payload, 8);
            if (secondDirection != 0)
            {
                directions.Add((Direction)secondDirection);
            }
        }
     
        float posX = BitConverter.ToSingle(payload, 12);
        float posY = BitConverter.ToSingle(payload, 16);

        // TODO: duplicate in ProcessPlayerInit
        // Room info
        PlayerInfo playerInfo = GameController.instance.playerInfo;
        playerInfo.PointInfo.HasTopConnection = BitConverter.ToBoolean(payload, 20);
        playerInfo.PointInfo.HasRightConnection = BitConverter.ToBoolean(payload, 24);
        playerInfo.PointInfo.HasBottomConnection = BitConverter.ToBoolean(payload, 28);
        playerInfo.PointInfo.HasLeftConnection = BitConverter.ToBoolean(payload, 32);
        playerInfo.PointInfo.IsExit = BitConverter.ToBoolean(payload, 36);
        playerInfo.PointInfo.IsSpawn = BitConverter.ToBoolean(payload, 40);

        OnMove(success, ListDirectionToVector(directions), new Vector2(posX, posY));
    }
}
