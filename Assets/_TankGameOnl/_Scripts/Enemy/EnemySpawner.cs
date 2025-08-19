using Lean.Pool;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : ZuSingleton<EnemySpawner>
{
    [SerializeField] protected List<Enemy> enemies;
    [SerializeField] protected float delay = 2f;
    private int enemyID;
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
            GameObject newEnemy = LeanPool.Spawn(this.GetRandomEnemy(), point.position, Quaternion.identity);
            Enemy enemy = newEnemy.GetComponent<Enemy>();
            enemy.Init(this.enemyID);
            NetworkServer.Spawn(newEnemy);

            yield return new WaitForSeconds(this.SpawnRate);
        }
    }

    private GameObject GetRandomEnemy()
    {
        float totalWeight = 100;
        float r = Random.Range(0, totalWeight);
        
        for (int i = 0; i < enemies.Count; i++)
        {
            if (r < enemies[i].weight) 
            {
                enemyID = i;
                enemies[i].gameObject.name = "Enemy";
                return enemies[i].gameObject;
            }

            r -= enemies[i].weight;
        }

        return null;
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
