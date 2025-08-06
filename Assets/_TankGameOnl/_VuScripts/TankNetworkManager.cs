using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankNetworkManager : NetworkManager
{
    #region Singleton
    protected static TankNetworkManager instance;
    public static TankNetworkManager Instance => instance;

    public override void Awake()
    {
        base.Awake();
        this.LoadInstance();
    }

    protected virtual void LoadInstance()
    {
        if (instance == null)
        {
            instance = this as TankNetworkManager;
            //DontDestroyOnLoad(gameObject);
            return;
        }

        if (instance != this) Debug.LogError("Another instance of Singleton already exits");
    }
    #endregion

    [Header("Tank Network Manager")]
    [SerializeField] protected EnemySpawner enemySpawner;
    [SerializeField] protected float timePerRound = 10f;
    [SerializeField] protected float timer = 0f;
    [SerializeField] protected int lev = 1;
    public int playerCount = 0;
    public int playerAlive= 0;


    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        this.playerCount++;
        this.playerAlive++;
        base.OnServerAddPlayer(conn);

        //enemySpawner.Spawning();

    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        this.playerCount--;

        if(conn.identity != null)
        {
            if (!conn.identity.GetComponent<Tank>().IsDeath)
            {
                this.playerAlive--;
            }
        }

        if (this.playerCount <= 0)
        {
            enemySpawner.StopSpawning();
            EnemySpawner.Instance.DespawnAllEnemies();
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
