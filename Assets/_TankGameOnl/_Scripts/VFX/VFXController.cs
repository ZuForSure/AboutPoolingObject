using UnityEngine;
using Mirror;
using Lean.Pool;
using System.Collections;

public enum VFXType
{
    Exploision = 0,
    Heal = 1,
    Buff = 2,
    GetHit = 3,
    EnemyDead = 4,
}

public class VFXController : NetworkBehaviour
{
    public VFXType type;

    private void OnEnable()
    {
        StartCoroutine(this.Despawn());
    }

    [Server]
    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(3);

        NetworkServer.UnSpawn(transform.gameObject);
        LeanPool.Despawn(transform.gameObject);
    }
}
