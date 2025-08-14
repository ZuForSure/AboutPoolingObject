using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    [Header("Follow Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float checkInterval = 1f;

    #region ServerCallBack
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
    #endregion

    void Update() => this.FollowPlayer();

    private void FixedUpdate()
    {
        //Loot at player
        if (this.player == null) return;
        this.AimTarget(this.player.transform.position);
    }

    protected void FindClosestPlayer()
    {
        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null) continue;
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

    void FollowPlayer()
    {
        if (!isServer) return;
        if (player == null) return;

        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    public void AimTarget(Vector3 target)
    {
        Vector2 diff = target - this.transform.position;
        diff = diff.normalized;
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90f);
    }
}
