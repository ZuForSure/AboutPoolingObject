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

    //void OnLevelChanged(int oldValue, int newValue)
    //{
    //    Debug.Log("Client received new difficulty level: " + newValue);
    //}

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
