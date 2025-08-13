using Mirror;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class ItemPickup : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnItemIdChanged))]
    [SerializeField] private int itemID;

    [SerializeField] private List<Sprite> itemSprites;

    private void OnItemIdChanged(int oldId, int newId)
    {
        SetSprite(newId);
    }

    [Server] 
    public void SetItemID(int id)
    {
        itemID = id;
        SetSprite(id);
    }

    private void SetSprite(int id)
    {
        if (id < 0 || id >= itemSprites.Count)
        {
            Debug.LogWarning($"Invalid item ID: {id}");
            return;
        }
        GetComponent<SpriteRenderer>().sprite = itemSprites[id];
    }

  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return;
      
        Tank tank = collision.GetComponent<Tank>();
        if (tank != null && tank.connectionToClient != null)
        {
            if (tank.inventory.Count >= tank.maxSlots) return;
            tank.TargetPlayPickupFly(tank.connectionToClient, itemID, transform.position);
            NetworkServer.Destroy(gameObject);
        }
    }
}
