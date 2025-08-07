using Mirror;
using Mirror.BouncyCastle.Math.Field;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;



[RequireComponent(typeof(NetworkIdentity))]
public class ItemPickup : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] private int itemID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return; // Chỉ server xử lý va chạm
        if (!collision.CompareTag("Player")) return; // Chỉ xử lý với Player
        Tank tank = collision.GetComponent<Tank>();
        if (tank != null && tank.connectionToClient != null)
        {
            if (tank.AddItem(tank.connectionToClient,itemID))
            {
                Debug.Log($"Player {collision.name} picked up item {itemID}");
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
