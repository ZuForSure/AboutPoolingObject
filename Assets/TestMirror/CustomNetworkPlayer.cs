using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkPlayer : MonoBehaviour
{
    public GameObject BULLET;
    public float moveSpeed = 5f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); 
        float moveY = Input.GetAxis("Vertical");   

        Vector3 moveDir = new Vector3(moveX, moveY, 0);
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var obj = Instantiate(BULLET);
            NetworkServer.Spawn(obj);
        }
    }
}
