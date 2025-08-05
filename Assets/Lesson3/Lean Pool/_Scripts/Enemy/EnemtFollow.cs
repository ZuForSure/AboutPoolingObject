using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemtFollow : NetworkBehaviour
{
    public GameObject player;     
    public float speed = 4f;     
    public float checkInterval = 1f;

    [ServerCallback]
    public override void OnStartServer()
    {
        base.OnStartServer();

        if (!isServer) return;
        InvokeRepeating(nameof(FindClosestPlayer), 0f, checkInterval);
    }

    protected virtual void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject player in players)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = player.transform;
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

        Vector3 direction = (player.transform.position - transform.parent.position).normalized;
        transform.parent.position += direction * speed * Time.deltaTime;
    }

}
