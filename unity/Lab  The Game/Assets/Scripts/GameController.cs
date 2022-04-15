using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

enum AuthServerCommandType
{
    Authenticate = 0,
    JoinLobby,
};

public class GameController : MonoBehaviour
{

    private Queue<Action> nextUpdateActions = new Queue<Action>();
    private Config config;
    private TCPClient client;
    private GameObject playerInst;
    private bool isGameStarted = false;
    private bool readyToStartGame = false;
    private Background background;
    private Player player;

    public static GameController instance;
    public uint roomSize = 0;
    public CommandsProcessor commandsProcessor;
    public PlayerInfo playerInfo;

    public GameObject PlayerPrefab;
    public GameObject BackgroundObject;
    public GameObject CameraObject;


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
    }

    void Start()
    {
        playerInfo = new PlayerInfo();
        config = Config.Load("CONFIG");

        ConnectToAuthServer();
        background = BackgroundObject.GetComponent<Background>();
    }

    void Update()
    {
        if (readyToStartGame == false)
        {
            return;
        }

        if ((readyToStartGame == true) && (isGameStarted == false))
        {
            StartGame();
            return;
        }

        ProcessUpdateActions();
    }
    private void StartGame()
    {
        isGameStarted = true;
        Debug.Log("Game started");

        playerInst = Instantiate(PlayerPrefab, new Vector2(0, 0), Quaternion.identity);
        player = playerInst.GetComponent<Player>();
        CameraObject.GetComponent<CameraBehaviour>().playerInst = playerInst;

        commandsProcessor = new CommandsProcessor(config);
        commandsProcessor.OnPlayerInit = OnPlayerInit;
        commandsProcessor.OnMove = OnPlayerMove;
        commandsProcessor.Start();
    }

    public void SendMove(Vector2 directions)
    {
        commandsProcessor.SendMove(directions);
    }

    public void UpdateBackgroud()
    {
        background.UpdateRoom(playerInfo.PointInfo);
    }

    private void OnPlayerMove(bool success, Vector2 directions, Vector2 position)
    {
        player.OnServerMove(position);
        OnNextUpdate(() =>
        {
            UpdateBackgroud();
        });
    }

    private void OnPlayerInit()
    {
        OnNextUpdate(() => {
            player.InitPosition(new Vector2(playerInfo.X, playerInfo.Y));
            background.Init(roomSize);
            UpdateBackgroud();
        });
    }

    private void OnReceiveFromAuthServer(byte[] command)
    {
        AuthServerCommandType commandType = (AuthServerCommandType)BitConverter.ToUInt32(command, 4);
        if (commandType == AuthServerCommandType.Authenticate)
        {
            SavePlayerTocken(command);
            JoinLobby();
        }
        else if (commandType == AuthServerCommandType.JoinLobby)
        {
            CheckJoinLobbyStatus(command);
        }
        else
        {
            Debug.Log("Unknown command type");
        }
    }

    private void ParseStartGameResponse(byte[] command)
    {
        Debug.Log("Got `StartGame` response");

        for (int i = 12; i < command.Length; i += 12)
        {
            uint playerId = BitConverter.ToUInt32(command, i);
            uint playerIndex = BitConverter.ToUInt32(command, i + 4);
            uint sessionIndex = BitConverter.ToUInt32(command, i + 8);

            Debug.Log(string.Format("Player {0} = player index: {1}, session index: {2}", playerId, playerIndex, sessionIndex));

            if (playerId == playerInfo.PlayerId)
            {
                playerInfo.PlayerIndex = playerIndex;
                playerInfo.SessionIndex = sessionIndex;
                break;
            }
        }

        readyToStartGame = true;
    }

    private void CheckJoinLobbyStatus(byte[] command)
    {
        uint statusCode = BitConverter.ToUInt32(command, 8);
        if (statusCode == 0)
        {
            Debug.Log("Join lobby error");
            return;
        }

        if (statusCode == 1)
        {
            playerInfo.PlayerId = BitConverter.ToUInt32(command, 12);
            Debug.Log("Joined lobby successfully. Player id: " + playerInfo.PlayerId);
            return;
        }

        if (statusCode == 2)
        {
            ParseStartGameResponse(command);
            return;
        }
    }

    private void SavePlayerTocken(byte[] command)
    {
        byte[] token = new byte[32];
        Array.Copy(command, 8, token, 0, 32);
        Debug.Log(string.Format("Set player token[{0}] to {1}", token.Length, BitConverter.ToString(token)));
        playerInfo.Token = token;
    }

    private void JoinLobby()
    {
        if (playerInfo.Token == null)
        {
            throw new Exception("Can't join lobby - no token");
        }

        byte[] joinLobbyCommand = new byte[40];

        // validation header
        byte[] validationHeaderBytes = BitConverter.GetBytes((uint)0xBADBEE);
        Array.Copy(validationHeaderBytes, 0, joinLobbyCommand, 0, validationHeaderBytes.Length);

        // command type
        byte[] commandTypeBytes = BitConverter.GetBytes((uint)AuthServerCommandType.JoinLobby);
        Array.Copy(commandTypeBytes, 0, joinLobbyCommand, 4, commandTypeBytes.Length);

        // payload tocken
        Array.Copy(playerInfo.Token, 0, joinLobbyCommand, 8, playerInfo.Token.Length);


        client.Send(joinLobbyCommand);
    }
    private void SendCredsToAuthServer()
    {
        byte[] loginCommand = new byte[68];

        // validation header
        byte[] validationHeaderBytes = BitConverter.GetBytes((uint)0xBADBEE);
        Array.Copy(validationHeaderBytes, 0, loginCommand, 0, validationHeaderBytes.Length);

        // command type
        byte[] commandTypeBytes = BitConverter.GetBytes((uint)AuthServerCommandType.Authenticate);
        Array.Copy(commandTypeBytes, 0, loginCommand, 4, commandTypeBytes.Length);

        // payload login
        byte[] tempBytes = Encoding.ASCII.GetBytes(config.login);
        byte[] loginBytes = new byte[36];
        Array.Copy(tempBytes, loginBytes, tempBytes.Length);
        Array.Copy(loginBytes, 0, loginCommand, 8, loginBytes.Length);

        // payload password
        tempBytes = Encoding.ASCII.GetBytes(config.password);
        byte[] passBytes = new byte[24];
        Array.Copy(tempBytes, passBytes, tempBytes.Length);
        Array.Copy(passBytes, 0, loginCommand, 44, passBytes.Length);

        client.Send(loginCommand);
    }

    private void ConnectToAuthServer()
    {
        client = new TCPClient(config.authServerAddr, (int)config.authServerPort);
        client.OnReceive = OnReceiveFromAuthServer;
        client.OnReady = SendCredsToAuthServer;
        client.Connect();
    }

    private void OnNextUpdate(Action action)
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
