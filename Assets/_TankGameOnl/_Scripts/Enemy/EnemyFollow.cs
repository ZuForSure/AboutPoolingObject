using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : NetworkBehaviour
{
    public GameObject player;     
    public float speed = 4f;     
    public float checkInterval = 1f;

    [ServerCallback]
    public override void OnStartServer()
    {
        base.OnStartServer();

        if (!NetworkServer.active) return;
        InvokeRepeating(nameof(FindClosestPlayer), 0f, checkInterval);
    }

    [ServerCallback]
    public override void OnStopServer()
    {
        base.OnStopServer();
        CancelInvoke(nameof(FindClosestPlayer));
    }

    protected void FindClosestPlayer()
    {
        float minDist = Mathf.Infinity;
        Transform closest = null;
        
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if(conn.identity == null) continue;
            if (!conn.identity.gameObject.activeSelf) 
            {
                player = null;
                continue;
            }

            GameObject playerObj = conn.identity.gameObject;
            float dist = Vector3.Distance(transform.position, playerObj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = playerObj.transform;
            }
        }

        if (closest != null)
        {
            player = closest.gameObject;
        }
    }

    void Update()
    {
        if (!isServer) return;
        if (player == null) return;

        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}
