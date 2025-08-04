using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] protected float timePerRound = 10f;
    [SerializeField] protected float timer = 0f;

    private void Update()
    {
        this.CheckTimer();
    }

    protected virtual void CheckTimer()
    {
        this.timer += Time.deltaTime;
        if (this.timer < timePerRound) return;

        if (SpawnEnemy.Instance.SpawnRate <= .5f)
        {
            SpawnEnemy.Instance.SpawnRate = .5f;
            this.timer = 0f;
            return;
        }

        SpawnEnemy.Instance.SpawnRate -= .5f;
        this.timer = 0f;    
    }
}
