using Lean.Pool;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : NetworkBehaviour
{
    [SerializeField] protected float delay = 2f;
    public float SpawnRate = 3f;
    [SerializeField] protected GameObject enemey;


    [Server]
    public virtual void Spawning()
    {
        StartCoroutine(this.Spawn());
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            Debug.Log("Spawn with rate: " + SpawnRate);
            Transform point = Point.Instance.GetRandomPoint();

            GameObject enemy = LeanPool.Spawn(enemey, point.position, Quaternion.identity);
            NetworkServer.Spawn(enemy);

            yield return new WaitForSeconds(this.SpawnRate);
        }
    }
}
