using Lean.Pool;
using UnityEngine;
using Mirror;
using Transform = UnityEngine.Transform;
using System;

[Serializable]
public class BulletDespawn
{
    [SerializeField] private Transform transform;
    public float speed = 10f;
    public float maxDistance = 50f;
    private Vector2 startPos;

    public void Init(Transform transform)
    {
        this.transform = transform;
        this.startPos = this.transform.position;
    }

    public void DespawnByDistance()
    {
        if (Vector2.Distance(startPos, transform.position) >= maxDistance)
        {
            NetworkServer.UnSpawn(transform.parent.gameObject);
            LeanPool.Despawn(transform.parent);
        }
    }
}
