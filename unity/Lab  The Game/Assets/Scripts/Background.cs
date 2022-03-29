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
    private Vector2 BLOCK_SCALE;

    public GameObject BlockPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Vector2 size = BlockPrefab.GetComponent<Renderer>().bounds.size;
        BLOCK_SCALE = new Vector2(
            GameController.instance.BLOCK_SIZE / size.x,
            GameController.instance.BLOCK_SIZE / size.y
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBlock(BlockType blockType)
    {
        int posX = 0;
        int posY = 0;
        switch (blockType)
        {
            case BlockType.Top: { posX = 0; posY = 1; break; }
            case BlockType.Bottom: { posX = 0; posY = -1; break; }
            case BlockType.Right: { posX = 1; posY = 0; break; }
            case BlockType.Left: { posX = -1; posY = 0; break; }
        }

        float x = posX * GameController.instance.BLOCK_SIZE;
        float y = posY * GameController.instance.BLOCK_SIZE;

        GameObject block = Instantiate(BlockPrefab, new Vector2(x, y), Quaternion.identity, transform);
        block.transform.localScale = BLOCK_SCALE;
    }

    public void OnPlayerMove(Vector2 moveDirections)
    {
        float x = -moveDirections.x;
        float y = -moveDirections.y;
        transform.Translate(x, y, 0);
    }
}
