using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using TMPro;

public class GameController : MonoBehaviour
{
    private LogRedirect logRedirect = new LogRedirect();
    private Queue<Action> nextUpdateActions = new Queue<Action>();
    private Communication communication;
    private PlayerInfo playerInfo;

    public static GameController instance;


    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

        logRedirect.Enable();
    }

    void Start()
    {
        playerInfo = new PlayerInfo();

        communication = new Communication(playerInfo);
        communication.Start();
    }

    void Update()
    {
        ProcessUpdateActions();
    }

    public void StartGame()
    {
        Debug.Log("Game Started");
    }

    public void OnNextUpdate(Action action)
    {
        nextUpdateActions.Enqueue(action);
    }

    private void ProcessUpdateActions()
    {
        foreach (Action action in nextUpdateActions)
        {
            action();
        }
        nextUpdateActions.Clear();
    }
}
