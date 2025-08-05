using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankNetworkManager : NetworkManager
{
    [Header("Tank Network Manager")]
    [SerializeField] protected EnemySpawner enemySpawner;
    [SerializeField] protected float timePerRound = 10f;
    [SerializeField] protected float timer = 0f;
    [SerializeField] protected int lev = 1;
    [SerializeField] protected int playerCount = 0;
    [SerializeField] protected bool canSpawnEnemy = false;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        this.playerCount++;
        base.OnServerAddPlayer(conn);

        if (canSpawnEnemy) return;
        enemySpawner.Spawning();
        canSpawnEnemy = true;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        this.playerCount--;

        if(this.playerCount == 0)
        {
            enemySpawner.StopSpawning();
            canSpawnEnemy = false;
        }

        base.OnServerDisconnect(conn);
    }

    public override void Update()
    {
        base.Update();

        if (NetworkServer.connections.Count == 0) return;
        this.CheckTimer();
    }

    [Server]
    void IncreaseLevel()
    {
        lev++;
        Debug.Log("Increased level to: " + lev);
    }

    protected void CheckTimer()
    {
        this.timer += Time.deltaTime;
        if (this.timer < timePerRound) return;

        if (enemySpawner.SpawnRate <= .5f)
        {
            enemySpawner.SpawnRate = .5f;
            this.timer = 0f;
            return;
        }

        enemySpawner.SpawnRate -= .5f;
        this.IncreaseLevel();
        this.timer = 0f;    
    }
}
