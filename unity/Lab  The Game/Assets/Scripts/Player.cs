using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 moveDirections = Vector2.zero;
    private Vector2 posFromServer = Vector2.zero;
    private bool isInit = false;
  
    private Rigidbody2D rb;
    //private ContactPoint2D[] collisionPoints = new ContactPoint2D[10];
    //private int collisionPointsCount = 0;

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
    public void OnServerMove(Vector2 position)
    {
        posFromServer = position;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

        if ((moveDirections.x != 0) || (moveDirections.y != 0))
        {
            GameController.instance.SendMove(moveDirections);
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
        float speed = GameController.instance.playerInfo.Speed;
        Vector2 step = new Vector2(direction.x * speed, direction.y * speed);
        rb.MovePosition(rb.position + step);
    }

}
