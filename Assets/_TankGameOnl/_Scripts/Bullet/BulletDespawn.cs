using Lean.Pool;
using UnityEngine;
using Mirror;

public class BulletDespawn : NetworkBehaviour
{
    public float speed = 10f;
    public float maxDistance = 50f;

    private Vector3 startPos;

    public override void OnStartServer()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (!isServer) return;

        //transform.position += transform.forward * speed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            NetworkServer.UnSpawn(transform.parent.gameObject);
            LeanPool.Despawn(transform.parent);
        }
    }
}