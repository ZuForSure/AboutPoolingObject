using Lean.Pool;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [Header("Fly")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private Vector2 flyDirec = Vector2.right;
    [Header("Despawn If Out Of Scene")]
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private Vector2 startPos;

    public override void OnStartServer()
    {
        startPos = transform.position;
    }

    void Update() => DespawnByDistance();

    void FixedUpdate() => Fly();

    protected void Fly()
    {
        if (!isServer) return;
        transform.Translate(flyDirec * this.speed * Time.fixedDeltaTime);
    }

    protected void DespawnByDistance()
    {
        if (!isServer) return;

        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            NetworkServer.UnSpawn(transform.gameObject);
            LeanPool.Despawn(transform);
        }
    }
}
