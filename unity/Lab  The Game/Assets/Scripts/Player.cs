using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float startMovingTime = 0;
    private bool isMoving = false;
    private List<MoveDirection> moveDirections;
    private CommandsProcessor commandsProcessor;

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
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        List<MoveDirection> directions = new List<MoveDirection>();

        if (horizontal == -1)
        {
            directions.Add(MoveDirection.Left);
        } 
        else if (horizontal == 1)
        {
            directions.Add(MoveDirection.Right);
        }

        if (vertical == -1)
        {
            directions.Add(MoveDirection.Top);
        }
        else if (vertical == 1)
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
            float x = 0;
            float y = 0;
            foreach (MoveDirection direction in moveDirections)
            {
                switch (direction)
                {
                    case MoveDirection.Left: x = -1; break;
                    case MoveDirection.Top: y = -1; break;
                    case MoveDirection.Right: x = 1; break;
                    case MoveDirection.Bottom: y = 1; break;
                }
            }
            transform.Translate(x, y, 0);
        }
    }
}
