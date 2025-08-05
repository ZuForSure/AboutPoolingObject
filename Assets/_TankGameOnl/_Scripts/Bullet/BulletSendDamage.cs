using Lean.Pool;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSendDamage : MonoBehaviour
{
    public int damage = 2;

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) return;

        EnemyTakeDamage enemyTakeDamage = collision.GetComponent<EnemyTakeDamage>();
        if (enemyTakeDamage == null)  return;

        this.SendDamage(enemyTakeDamage);
        this.DespawnBullet();
    }

    protected void SendDamage(EnemyTakeDamage enemyTakeDamage)
    {
        enemyTakeDamage.DeductHP(this.damage);
    }

    protected void DespawnBullet()
    {
        NetworkServer.UnSpawn(transform.parent.gameObject);
        LeanPool.Despawn(transform.parent);
    }
}
