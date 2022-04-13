using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject playerInst = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        if (playerInst == null)
        {
            return;
        }

        transform.position = new Vector3(playerInst.transform.position.x, playerInst.transform.position.y, transform.position.z);
    }
}
