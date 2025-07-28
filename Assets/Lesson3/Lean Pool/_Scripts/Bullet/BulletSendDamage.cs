using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSendDamage : MonoBehaviour
{
    public int damage = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")) return;

        EnemyTakeDamage enemyTakeDamage = collision.GetComponent<EnemyTakeDamage>();
        if (enemyTakeDamage == null)  return;

        this.SendDamage(enemyTakeDamage);
        this.DespawnBullet();
    }

    protected virtual void SendDamage(EnemyTakeDamage enemyTakeDamage)
    {
        enemyTakeDamage.DeductHP(this.damage);
    }

    protected virtual void DespawnBullet()
    {
        LeanPool.Despawn(transform.parent);
    }
}
