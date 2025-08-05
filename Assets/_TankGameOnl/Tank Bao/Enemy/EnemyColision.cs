using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColision : NetworkBehaviour
{
    private float damageCooldown = 0.5f;
    private bool isEnemyColision = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"IsServer: {isServer} | IsClient: {isClient}");
        if (!isServer) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            TankHeal tankHeal = collision.gameObject.GetComponentInChildren<TankHeal>();
            if (tankHeal == null)
            {
                Debug.LogWarning("Không tìm thấy TankHeal trong Player");
            }
            else
            {
                if (!isEnemyColision)
                {
                    Debug.Log("ád");
                    tankHeal.ReduceHeal(1);
                    isEnemyColision = true;
                    StartCoroutine(SetValueEnemyColision());
                }


            }
        }

    }

    IEnumerator SetValueEnemyColision()
    {
        yield return new WaitForSeconds(damageCooldown);
        isEnemyColision = false;
    }




}
