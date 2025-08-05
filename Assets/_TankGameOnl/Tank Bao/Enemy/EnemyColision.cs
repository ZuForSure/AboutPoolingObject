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
        Debug.Log($"OnCollisionEnter2D - IsServer: {NetworkServer.active} | IsClient: {NetworkClient.active}");
        if (!NetworkServer.active) return;
        Debug.Log($"Collided with: {collision.gameObject.name}, tag: {collision.gameObject.tag}");

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"OnCollisionEnter2D - Collision with Player: {collision.gameObject.name}");
            if (!collision.gameObject.TryGetComponent<Tank>(out var tank))
            {
                Debug.LogWarning("Không tìm thấy TankHeal trong Player");
            }
            else
            {
                //if (!isEnemyColision)

                Debug.Log("ád");
                tank.TankHeal.ReduceHeal(1);
                isEnemyColision = true;
                //StartCoroutine(SetValueEnemyColision());

            }
        }

    }

    //IEnumerator SetValueEnemyColision()
    //{
    //    yield return new WaitForSeconds(damageCooldown);
    //    isEnemyColision = false;
    //}




}
