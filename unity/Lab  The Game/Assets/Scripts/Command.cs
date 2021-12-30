using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    private CommandType commandType;
    private uint        sessionIndex;
    private uint        playerIndex;

    public Command(CommandType commandType, uint sessionIndex, uint playerIndex)
    {
        this.commandType = commandType;
        this.sessionIndex = sessionIndex;
        this.playerIndex = playerIndex;
    }
}
