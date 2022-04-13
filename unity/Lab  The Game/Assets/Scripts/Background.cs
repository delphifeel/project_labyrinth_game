using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Center,
    Top,
    Right,
    Bottom,
    Left,
}

public class Background : MonoBehaviour
{
    public GameObject BlockPrefab;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuildWallsAroundPlayer(PlayerInfo playerInfo)
    {
        Vector2 wallSize = BlockPrefab.GetComponent<Renderer>().bounds.size;
        float ROOM_SIZE = GameController.instance.RoomSize;
        // top wall
        if (!playerInfo.PointInfo.HasTopConnection)
        {
            _AddWall(new Vector2(0, ROOM_SIZE / 2 + wallSize.y / 2),
                    new Vector2(ROOM_SIZE / wallSize.x, 1));
        }
        // right wall
        if (!playerInfo.PointInfo.HasRightConnection)
        {
            _AddWall(new Vector2(ROOM_SIZE / 2 + wallSize.x / 2, 0),
                    new Vector2(1, ROOM_SIZE / wallSize.y));
        }
        // bottom wall
        if (!playerInfo.PointInfo.HasBottomConnection)
        {
            _AddWall(new Vector2(0, -ROOM_SIZE / 2 - wallSize.y / 2),
                     new Vector2(ROOM_SIZE / wallSize.x, 1));
        }
        // left wall
        if (!playerInfo.PointInfo.HasLeftConnection)
        {
            _AddWall(new Vector2(-ROOM_SIZE / 2 - wallSize.x / 2, 0),
                    new Vector2(1, ROOM_SIZE / wallSize.y));
        }
    }

    private void _AddWall(Vector2 pos, Vector2 scale)
    {
        GameObject block = Instantiate(BlockPrefab, pos, Quaternion.identity, transform);
        block.transform.localScale = scale;
    }
}
