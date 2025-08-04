using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : ZuSingleton<SpawnEnemy>
{
    [SerializeField] protected float delay = 2f;
    public float SpawnRate = 3f;
    [SerializeField] protected GameObject enemey;

    //private void Start() => InvokeRepeating(nameof(this.Spawn), this.delay, this.SpawnRate);
    private void Start() => StartCoroutine(this.Spawn());

    protected IEnumerator Spawn()
    {
        while (true)
        {
            Debug.Log("Spawn with rate: " + SpawnRate);
            Transform point = Point.Instance.GetRandomPoint();
            LeanPool.Spawn(enemey, point.position, Quaternion.identity);

            yield return new WaitForSeconds(this.SpawnRate);
        }
    }
}
