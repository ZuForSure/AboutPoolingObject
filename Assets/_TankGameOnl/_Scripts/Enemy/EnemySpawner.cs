using Lean.Pool;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : ZuSingleton<EnemySpawner>
{
    [SerializeField] protected GameObject enemy;
    [SerializeField] protected float delay = 2f;
    private Coroutine coroutine;

    public bool CanSpawnEnemy = true;
    public float SpawnRate = 5f, MaxRate = 5f;
    public int EnemyPerRound = 4;

    [Server]
    public void Spawning()
    {
        if (!CanSpawnEnemy) return;
        CanSpawnEnemy = false;

        this.SpawnRate = MaxRate;
        coroutine = StartCoroutine(this.Spawn());
    }

    [Server]
    public void StopSpawning()
    {
        CanSpawnEnemy = true;

        if (coroutine == null) return;
        StopCoroutine(coroutine);
        coroutine = null;
    }

    public IEnumerator Spawn()
    {
        while (true)
        {
            Transform point = Point.Instance.GetRandomPoint();

            GameObject newEnemy = LeanPool.Spawn(this.enemy, point.position, Quaternion.identity);
            NetworkServer.Spawn(newEnemy);

            yield return new WaitForSeconds(this.SpawnRate);
        }
    }

    public void DespawnAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            NetworkServer.UnSpawn(enemy);
            LeanPool.Despawn(enemy);
        }
    }

    public void CheckCanContinueSpawn()
    {
        if(TankNetworkManager.Instance.Players.Count <= 0)
        {
            this.StopSpawning();
        }
    }
}
