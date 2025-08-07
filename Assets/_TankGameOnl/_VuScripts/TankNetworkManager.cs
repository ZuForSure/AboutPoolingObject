using Mirror;
using System;
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
        NetworkServer.RegisterHandler<ClientRequestSever>(OnClientRequestServer);
    }

    private void OnClientRequestServer(NetworkConnectionToClient client, ClientRequestSever sever)
    {
        SeverSendMessage report = new SeverSendMessage
        {
            severTime = NetworkTime.time
        };
        client.Send(report); // G?i th?i gian máy ch? hi?n t?i ??n client v?a g?i message cho máy ch?
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
    [SerializeField] protected float timePerRound = 15f;
    [SerializeField] protected float timer = 0f;
    [SerializeField] protected int lev = 1;
    public int playerCount = 0;
    public List<GameObject> Players;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        playerCount++;
        this.Players.Add(conn.identity.gameObject);

        if (TankGameManager.Instance.IsPlaying)
        {
            conn.identity.GetComponent<Tank>().IsReady = true;
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        this.playerCount--;

        if (conn.identity != null)
        {
            if (!conn.identity.gameObject.GetComponent<Tank>().IsDeath)
            {
                this.Players.Remove(conn.identity.gameObject);
            }
        }

        if (Players.Count <= 0 && playerCount <= 0)
        {
            enemySpawner.StopSpawning();
            EnemySpawner.Instance.DespawnAllEnemies();
        }

        base.OnServerDisconnect(conn);
    }



    public override void Update()
    {
        base.Update();

        if (NetworkServer.connections.Count == 0 || this.playerCount == 0) return;
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
