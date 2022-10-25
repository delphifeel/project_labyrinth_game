using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    SpriteRenderer imgRenderer;
    bool prevIsExit = false;

    // Start is called before the first frame update
    void Start()
    {
        imgRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var pointInfo = GameController.instance.PlayerInfo.PointInfo;
        if (prevIsExit != pointInfo.IsExit)
        {
            imgRenderer.enabled = pointInfo.IsExit;
            prevIsExit = pointInfo.IsExit;
        }
    }
}
