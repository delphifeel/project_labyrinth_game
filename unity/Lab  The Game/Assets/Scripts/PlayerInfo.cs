using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LabPointType
{
    public uint Id;
    public uint TopConnectionId;
    public uint RightConnectionId;
    public uint BottomConnectionId;
    public uint LeftConnectionId;
    public bool IsExit;
    public bool IsSpawn;
}

public class PlayerInfo
{
    public uint SessionIndex = 0;
    public uint PlayerIndex = 0;
    public uint PlayerId = 0;
    public byte[] Token = null;

    public int X = 0;
    public int Y = 0;
    public LabPointType LabPoint;
}
