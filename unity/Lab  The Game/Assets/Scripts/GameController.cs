using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using TMPro;

public class GameController : MonoBehaviour
{
    readonly UpdateQueue updateQueue = new();
    readonly LogRedirect logRedirect = new();
    readonly InputProcessor inputProcessor = new();
    readonly Communication communication = new();

    public TurnState TurnState { get; } = new();
    public PlayerInfo PlayerInfo { get; } = new();

    public static GameController instance;
    public GameObject Visuals;


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
        TurnState.Reset();
        communication.Init(TurnState);
        communication.Start();
    }

    void Update()
    {
        updateQueue.Process();
        inputProcessor.Process();
    }

    public void StartGame()
    {
        Debug.Log("Game Started");
        updateQueue.OnNextUpdate(() =>
        {
            Visuals.SetActive(true);
        });
    }
}
