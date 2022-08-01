using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogRedirect
{
    readonly string debugFile = "debug.log";

    public void Enable()
    {
        Application.logMessageReceivedThreaded += _HandleLog;
    }

    public void Disable()
    {
        Application.logMessageReceivedThreaded -= _HandleLog;
    }

    private void _HandleLog(string logString, string stackTrace, LogType type)
    {

        using (StreamWriter sw = new StreamWriter(debugFile, true))
        {
            sw.WriteLineAsync(logString);
        }
    }
}
