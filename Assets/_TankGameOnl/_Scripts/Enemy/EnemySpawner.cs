using Lean.Pool;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : ZuSingleton<EnemySpawner>
{
    [SerializeField] protected float delay = 2f;
    public float SpawnRate = 3f, maxRate = 3f;
    public Coroutine coroutine;
    [SerializeField] protected GameObject enemey;


    [Server]
    public void Spawning()
    {
        this.SpawnRate = maxRate;
        coroutine = StartCoroutine(this.Spawn());
    }

    [Server]
    public void StopSpawning()
    {
        if (coroutine == null) return;
        StopCoroutine(coroutine);
        coroutine = null;
    }

    public IEnumerator Spawn()
    {
        while (true)
        {
            Transform point = Point.Instance.GetRandomPoint();

            GameObject enemy = LeanPool.Spawn(enemey, point.position, Quaternion.identity);
            NetworkServer.Spawn(enemy);

            yield return new WaitForSeconds(this.SpawnRate);
        }
    }
}
