using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
      
        BoatController tank = collision.GetComponent<BoatController>();
        if (tank != null && tank.connectionToClient != null)
        {
            if (tank.inventory.Count >= tank.maxSlots) return;
            tank.TargetPlayPickupFly(tank.connectionToClient, itemID, transform.position);
            NetworkServer.Destroy(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{eventData.pointerClick.name}");
    }
}
