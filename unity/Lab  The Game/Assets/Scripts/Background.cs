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
    private uint roomSize;
    private GameObject topWall;
    private GameObject rightWall;
    private GameObject bottomWall;
    private GameObject leftWall;
    private GameObject exit;

    public GameObject BlockPrefab;
    public GameObject FloorPrefab;
    public GameObject ExitPrefab;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(uint roomSize)
    {
        this.roomSize = roomSize;
        _BuildFloor(new Vector2(0, 0));
        _BuildWalls();
    }

    public void UpdateRoom(PointInfoType room)
    {
        // walls
        if (topWall.activeSelf == room.HasTopConnection)
            topWall.SetActive(!room.HasTopConnection);
        if (rightWall.activeSelf == room.HasRightConnection)
            rightWall.SetActive(!room.HasRightConnection);
        if (bottomWall.activeSelf == room.HasBottomConnection)
            bottomWall.SetActive(!room.HasBottomConnection);
        if (leftWall.activeSelf == room.HasLeftConnection)
            leftWall.SetActive(!room.HasLeftConnection);

        // exit
        if (room.IsExit && !_ExitVisible())
            _ShowExit();
        else if (!room.IsExit && _ExitVisible())
            _HideExit();
    }

    private void _BuildWalls()
    {
        Vector2 wallSize = BlockPrefab.GetComponent<Renderer>().bounds.size;
        float ROOM_SIZE = roomSize;

        // top wall
        topWall = _CreateWall(new Vector2(0, ROOM_SIZE / 2 + wallSize.y / 2),
                           new Vector2(ROOM_SIZE / wallSize.x, 1));
        topWall.SetActive(false);
        // right wall
        rightWall = _CreateWall(new Vector2(ROOM_SIZE / 2 + wallSize.x / 2, 0),
                             new Vector2(1, ROOM_SIZE / wallSize.y));
        rightWall.SetActive(false);
        // bottom wall
        bottomWall = _CreateWall(new Vector2(0, -ROOM_SIZE / 2 - wallSize.y / 2),
                              new Vector2(ROOM_SIZE / wallSize.x, 1));
        bottomWall.SetActive(false);
        // left wall
        leftWall = _CreateWall(new Vector2(-ROOM_SIZE / 2 - wallSize.x / 2, 0),
                            new Vector2(1, ROOM_SIZE / wallSize.y));
        leftWall.SetActive(false);
    }

    private void _BuildFloor(Vector2 pos)
    {
        Vector2 floorSize = FloorPrefab.GetComponent<Renderer>().bounds.size;
        float ROOM_SIZE = roomSize;
        GameObject floor = Instantiate(FloorPrefab, pos, Quaternion.identity, transform);
        floor.transform.localScale = new Vector2(ROOM_SIZE / floorSize.x, ROOM_SIZE / floorSize.y);
    }

    private GameObject _CreateWall(Vector2 pos, Vector2 scale)
    {
        GameObject block = Instantiate(BlockPrefab, pos, Quaternion.identity, transform);
        block.transform.localScale = scale;
        return block;
    }

    private void _ShowExit()
    {
        if (exit != null)
        {
            exit.SetActive(true);
            return;
        }
        Vector2 pos = Vector2.zero;
        Vector2 exitSize = ExitPrefab.GetComponent<Renderer>().bounds.size;
        exit = Instantiate(ExitPrefab, pos, Quaternion.identity, transform);
        float scale = roomSize / 4 / exitSize.y;
        exit.transform.localScale = new Vector2(scale, scale);
    }

    private void _HideExit()
    {
        if (exit == null)
            return;

        exit.SetActive(false);
    }
    
    private bool _ExitVisible()
    {
        return (exit != null) && (exit.activeSelf);
    }
}
