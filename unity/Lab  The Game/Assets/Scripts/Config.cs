using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Config
{
    public string authServerAddr;
    public string gameServerAddr;
    public uint authServerPort;
    public uint gameServerPort;
    public string login;
    public string password;

    static public Config Load(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        if (textAsset == null)
        {
            throw new Exception("Can't find " + fileName);
        }

        Config instance = JsonUtility.FromJson<Config>(textAsset.text);
        if (instance == null)
        {
            throw new Exception("Cant parse " + fileName);
        }

        return instance;
    }
}
