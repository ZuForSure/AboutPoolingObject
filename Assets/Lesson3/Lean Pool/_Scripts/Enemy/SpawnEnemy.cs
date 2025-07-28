using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] protected float delay = 2f;
    [SerializeField] protected float rate = 1f;
    [SerializeField] protected GameObject enemey;

    private void Start() => InvokeRepeating(nameof(this.Spawn), this.delay, this.rate);

    protected virtual void Spawn()
    {
        Transform point = Point.Instance.GetRandomPoint();
        LeanPool.Spawn(enemey, point.position, Quaternion.identity);
    }
}
