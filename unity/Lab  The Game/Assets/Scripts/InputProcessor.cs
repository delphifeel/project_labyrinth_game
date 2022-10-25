using System.Collections.Generic;
using UnityEngine;

public class InputProcessor
{
    public void Process()
    {
        var (horizontalMove, verticalMove) = CalcMoveAxis();
        ProcessMove(horizontalMove, verticalMove); 
    }

    private (float, float) CalcMoveAxis()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            return (0, 1);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            return (0, -1);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            return (1, 0);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            return (-1, 0);
        }
        return (0, 0);
    }

    private void ProcessMove(float horizontalMove, float verticalMove)
    {
        var turnState = GameController.instance.TurnState;
        var pointInfo = GameController.instance.PlayerInfo.PointInfo;

        if (horizontalMove > 0)
        {
            if (turnState.IsPlayerMoving && turnState.PlayerMoveDirection == Direction.Left)
            {
                turnState.CancelPlayerMove();
                return;
            }

            if (!pointInfo.HasRightConnection)
            {
                return;
            }

            turnState.MovePlayer(Direction.Right);
            return;
        }

        if (horizontalMove < 0)
        {
            if (turnState.IsPlayerMoving && turnState.PlayerMoveDirection == Direction.Right)
            {
                turnState.CancelPlayerMove();
                return;
            }

            if (!pointInfo.HasLeftConnection)
            {
                return;
            }

            turnState.MovePlayer(Direction.Left);
            return;
        }
        if (verticalMove > 0)
        {
            if (turnState.IsPlayerMoving && turnState.PlayerMoveDirection == Direction.Bottom)
            {
                turnState.CancelPlayerMove();
                return;
            }

            if (!pointInfo.HasTopConnection)
            {
                return;
            }

            turnState.MovePlayer(Direction.Top);
            return;
        }

        if (verticalMove < 0)
        {
            if (turnState.IsPlayerMoving && turnState.PlayerMoveDirection == Direction.Top)
            {
                turnState.CancelPlayerMove();
                return;
            }

            if (!pointInfo.HasBottomConnection)
            {
                return;
            }

            turnState.MovePlayer(Direction.Bottom);
        }
    }
}

