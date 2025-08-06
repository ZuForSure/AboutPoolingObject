using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankGameManager : ZuSingleton<TankGameManager>
{
    [SerializeField] private int heal; public int Heal => heal;

    public bool IsPlaying => !EnemySpawner.Instance.CanSpawnEnemy;

    public bool CheckAllPlayersReady()
    {
        bool allReady = true;
        int count = 0;
        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null) continue;
            Tank tank = conn.identity.GetComponent<Tank>();
            if (tank == null || !tank.IsReady)
            {
                allReady = false;
                break;
            }
            else
            {
                count++;
            }
        }
        Debug.Log($"Check all: allReady: {allReady}, count: {count}");

        if (allReady && count >= TankNetworkManager.Instance.playerCount)
        {
            Debug.Log("All players are ready. Starting the game...");

            StartGame();
            // Here you can add logic to start the game, e.g., spawning enemies, etc.
        }
        else
        {
            Debug.Log("Not all players are ready yet.");
        }
        return allReady;
    }
    private void StartGame()
    {
        // Logic to start the game, e.g., spawning enemies, initializing game state, etc.
        Debug.Log("Game started!");

        EnemySpawner.instance.Spawning();
    }
}
