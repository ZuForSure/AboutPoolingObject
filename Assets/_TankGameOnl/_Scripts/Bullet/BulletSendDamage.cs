using Lean.Pool;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletSendDamage : MonoBehaviour
{
    public int damage;

    private void Start()
    {
        SetBulletDamage();
        TankGameManager.Instance.OnSendDamagedEvent += OnReceiverDamageEvent;
    }

   

    private void OnDestroy()
    {
        TankGameManager.Instance.OnSendDamagedEvent -= OnReceiverDamageEvent;
    }

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
        Debug.Log($"SendDamage : {damage}");
        VFXSpawner.Instance.Spawning(VFXType.Exploision, transform.position, transform.rotation);
    }

    protected void DespawnBullet()
    {
        NetworkServer.UnSpawn(transform.parent.gameObject);
        LeanPool.Despawn(transform.parent);
    }

    
    
    private void SetBulletDamage()
    {
        if (!NetworkServer.active) return;
        Debug.Log($"SetBulletDamage");
        this.damage = TankGameManager.Instance.Damaged;
    }
    private void OnReceiverDamageEvent(int obj)
    {
        damage = obj;
    }

}
