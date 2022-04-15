using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PointInfoType
{
    public bool HasTopConnection;
    public bool HasRightConnection;
    public bool HasBottomConnection;
    public bool HasLeftConnection;
    public bool IsExit;
    public bool IsSpawn;
}

public class PlayerInfo
{
    public uint SessionIndex = 0;
    public uint PlayerIndex = 0;
    public uint PlayerId = 0;
    public byte[] Token = null;

    public float X = 0;
    public float Y = 0;
    public float Speed = 0;
    public PointInfoType PointInfo;
}
