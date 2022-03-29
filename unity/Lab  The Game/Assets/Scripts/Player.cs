using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 moveDirections = new Vector2(0, 0); 
    private CommandsProcessor commandsProcessor;

    public Action<Vector2> OnMove = null;

    // Start is called before the first frame update
    void Start()
    {
        commandsProcessor = GameController.instance.commandsProcessor;
        commandsProcessor.OnMove = OnSendMove;
    }

    // Update is called once per frame
    void Update()
    {
        ProccessMovement();
    }

    private void ProccessMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDirections.x = -1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDirections.x = 1;
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            moveDirections.x = 0;
        }


        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDirections.y = 1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDirections.y = -1;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            moveDirections.y = 0;
        }

        if ((moveDirections.x != 0) || (moveDirections.y != 0))
        {
            commandsProcessor.SendMove(moveDirections);
            OnMove(moveDirections);
        }
    }

    private void OnSendMove(Vector2 moveDirections)
    {
        //this.moveDirections = moveDirections;
    }
}
