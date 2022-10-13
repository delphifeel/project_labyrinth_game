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
    // TODO: empty mocked token
    public byte[] Token = new byte[32];

    public PointInfoType PointInfo;

    override public string ToString()
    {
        return string.Format("IsExit={0}, IsSpawn={1}, Connections = [{2}, {3}, {4}, {5}]", 
            PointInfo.IsExit,
            PointInfo.IsSpawn,
            PointInfo.HasTopConnection,
            PointInfo.HasRightConnection,
            PointInfo.HasBottomConnection,
            PointInfo.HasLeftConnection
        );
    }
}
