using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateQueue
{
    Queue<Action> nextUpdateActions = new Queue<Action>();

    public void OnNextUpdate(Action action)
    {
        nextUpdateActions.Enqueue(action);
    }

    public void Process()
    {
        foreach (Action action in nextUpdateActions)
        {
            action();
        }
        nextUpdateActions.Clear();
    }
}
