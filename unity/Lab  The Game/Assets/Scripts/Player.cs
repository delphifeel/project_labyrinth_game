using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool isMoving = false;
    private List<MoveDirection> moveDirections;
    private CommandsProcessor commandsProcessor;

    public Action<List<MoveDirection>> OnMove = null;

    // Start is called before the first frame update
    void Start()
    {
        commandsProcessor = GameController.instance.commandsProcessor;
    }

    // Update is called once per frame
    void Update()
    {
        ProccessMovement();
    }

    private void ProccessMovement()
    {
        List<MoveDirection> directions = new List<MoveDirection>();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            directions.Add(MoveDirection.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            directions.Add(MoveDirection.Right);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            directions.Add(MoveDirection.Top);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            directions.Add(MoveDirection.Bottom);
        }

        if (directions.Count > 0)
        {
            commandsProcessor.SendMove(directions, (List<MoveDirection> d) =>
            {
                isMoving = true;
                moveDirections = d;
            });
        }

        if (isMoving)
        {
            isMoving = false;
            OnMove(moveDirections);
            
        }
    }
}
