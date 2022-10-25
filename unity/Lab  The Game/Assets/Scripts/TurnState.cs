using System;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Top = 1,
    Right,
    Bottom,
    Left,
}
public class TurnState 
{
    readonly List<Action> listeners = new();
    readonly List<Action> notResetListeners = new();
    public bool IsPlayerMoving { get; private set; }
    public Direction PlayerMoveDirection { get; private set; }

    public void Reset()
    {
        IsPlayerMoving = false;
        NotifyListeners(true);
    }

    public void OnChange(Action action)
    {
        listeners.Add(action);
    }

    public void OnNotResetChange(Action action)
    {
        notResetListeners.Add(action);
    }

    public void MovePlayer(Direction direction)
    {
        IsPlayerMoving = true;
        PlayerMoveDirection = direction;
        Debug.Log(string.Format("Move {0}", direction));
        NotifyListeners();
    }

    public void CancelPlayerMove()
    {
        IsPlayerMoving = false;
        Debug.Log("Cancel move");
        NotifyListeners();
    }

    private void NotifyListeners(bool reset = false)
    {
        foreach (var action in listeners)
        {
            action();
        }

        if (!reset)
        {
            foreach (var action in notResetListeners)
            {
                action();
            }
        }
    }
}
