using Lean.Pool;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColision : MonoBehaviour
{
    private float damageCooldown = 0.5f;
    private bool isEnemyColision = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!NetworkServer.active) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (!collision.gameObject.TryGetComponent<BoatController>(out var tank))
            {
                Debug.LogWarning("Không tìm thấy TankHeal trong Player");
            }
            else
            {
                if(isEnemyColision) return;
                tank.BoatHeal.ReduceHeal(1);
                isEnemyColision = true;
                StartCoroutine(SetValueEnemyColision());
            }
        }
    }

    IEnumerator SetValueEnemyColision()
    {
        yield return new WaitForSeconds(damageCooldown);
        isEnemyColision = false;

        NetworkServer.UnSpawn(transform.parent.gameObject);
        LeanPool.Despawn(transform.parent.gameObject);
    }
}
