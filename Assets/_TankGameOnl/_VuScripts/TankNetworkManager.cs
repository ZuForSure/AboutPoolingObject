using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankNetworkManager : NetworkManager
{
    [Header("Tank Network Manager")]
    [SerializeField] protected SpawnEnemy spawnEnemy;
    [SerializeField] protected float timePerRound = 10f;
    [SerializeField] protected float timer = 0f;
    [SerializeField] protected int lev = 1;
    [SerializeField] protected bool canSpawnEnemy = false;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        Debug.Log("Test");
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);

        if (canSpawnEnemy) return;

        spawnEnemy.Spawning();
        canSpawnEnemy = true;
    }

    public override void Update()
    {
        base.Update();

        this.CheckTimer();
    }

    [Server]
    void IncreaseLevel()
    {
        lev++;
        Debug.Log("Increased level to: " + lev);
    }

    protected virtual void CheckTimer()
    {
        this.timer += Time.deltaTime;
        if (this.timer < timePerRound) return;

        if (spawnEnemy.SpawnRate <= .5f)
        {
            spawnEnemy.SpawnRate = .5f;
            this.timer = 0f;
            return;
        }

        spawnEnemy.SpawnRate -= .5f;
        this.IncreaseLevel();
        this.timer = 0f;    
    }
}
