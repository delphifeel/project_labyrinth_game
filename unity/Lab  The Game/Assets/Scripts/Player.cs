using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 collisionVelocity = new Vector2(0, 0);
    private Vector2 moveDirections = new Vector2(0, 0);
    private Vector2 posFromServer = Vector2.zero;
    private bool isInit = false;
  
    private CommandsProcessor commandsProcessor;
    private Rigidbody2D rb;
    private ContactPoint2D[] collisionPoints = new ContactPoint2D[10];
    private int collisionPointsCount = 0;

    public void InitPosition(Vector2 newPosition)
    {
        if (isInit)
        {
            Debug.LogWarning("Already init position");
            return;
        }
        isInit = true;
        rb.MovePosition(newPosition);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        commandsProcessor = GameController.instance.commandsProcessor;
        commandsProcessor.OnMove = OnMoveFromServer;
    }

    private void Update()
    {
        ProccessMovement();
    }

    private void FixedUpdate()
    {
        if (posFromServer != Vector2.zero)
        {
            rb.MovePosition(posFromServer);
            posFromServer = Vector2.zero;
            return;
        }

        if (collisionVelocity != Vector2.zero)
        {
            moveDirections += collisionVelocity;
        }

        if ((moveDirections.x != 0) || (moveDirections.y != 0))
        {
            commandsProcessor.SendMove(moveDirections);
            Move(moveDirections);
        }
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
    }

    private void Move(Vector2 direction)
    {
        rb.MovePosition(rb.position + direction);
    }

    private void OnMoveFromServer(bool success ,Vector2 moveDirections, Vector2 position)
    {
        posFromServer = position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
      
    }
}
