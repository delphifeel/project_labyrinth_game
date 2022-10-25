using System.Collections.Generic;
using UnityEngine;


public class Arrows : MonoBehaviour
{
    readonly UpdateQueue updateQueue = new();
    readonly Dictionary<Direction, GameObject> arrows = new();

    // Start is called before the first frame update
    void Start()
    {
        arrows[Direction.Top] = GetArrow("ArrowTop");
        arrows[Direction.Right] = GetArrow("ArrowRight");
        arrows[Direction.Bottom] = GetArrow("ArrowBottom");
        arrows[Direction.Left] = GetArrow("ArrowLeft");

        var turnState = GameController.instance.TurnState;
        turnState.OnChange(() =>
        {
            UpdateActiveArrows();
        });
    }

    // Update is called once per frame
    void Update()
    {
        var point = GameController.instance.PlayerInfo.PointInfo;
        SetArrowVisible(Direction.Top, point.HasTopConnection);
        SetArrowVisible(Direction.Right, point.HasRightConnection);
        SetArrowVisible(Direction.Bottom, point.HasBottomConnection);
        SetArrowVisible(Direction.Left, point.HasLeftConnection);
        updateQueue.Process();
    }

    private void UpdateActiveArrows()
    {
        updateQueue.OnNextUpdate(() =>
        {
            var turnState = GameController.instance.TurnState;
            foreach (var kv in arrows)
            {
                if (!turnState.IsPlayerMoving)
                {
                    SetArrowActive(kv.Value, false);
                    continue;
                }

                SetArrowActive(kv.Value, turnState.PlayerMoveDirection == kv.Key);
            }
        });
    }

    private void SetArrowActive(GameObject arrow, bool value)
    {
        if (!arrow.activeSelf)
        {
            return;
        }
        var renderer = arrow.GetComponent<SpriteRenderer>();
        renderer.color = value ? Color.yellow: Color.white;
    }

    private void SetArrowVisible(Direction direction, bool value)
    {
        arrows[direction].SetActive(value);
    }

    private GameObject GetArrow(string name)
    {
        return transform.Find(name).gameObject;
    }
}
