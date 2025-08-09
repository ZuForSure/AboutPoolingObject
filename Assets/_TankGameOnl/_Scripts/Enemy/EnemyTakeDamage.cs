using Lean.Pool;
using Mirror;
using Mirror.BouncyCastle.Security;
using UnityEngine;

public class EnemyTakeDamage : MonoBehaviour
{
    public int maxHp = 10;
    public int currentHP = 10;
    public int exp;

    private void OnEnable()
    {
        this.currentHP = this.maxHp;
    }

    public virtual void DeductHP(int amount)
    {
        if (this.currentHP == 0)
        {
            this.DeSpawnEnemey();
            DropItem();
        }

        this.currentHP -= amount;
    }

    [Server]
    protected virtual void DeSpawnEnemey()
    {
        NetworkServer.UnSpawn(transform.parent.gameObject);
        LeanPool.Despawn(transform.parent);
        LevelManager.Instance.AddExp(exp);
    }
    [Server]
    private void DropItem()
    {
        Debug.Log($"DropItem - Sever : {NetworkServer.active} - Client : {NetworkClient.active}");
        int randomDropItem = Random.Range(0, 100);
        Debug.Log($"randomDropItem : {randomDropItem}");
        if (randomDropItem <= 90)
        {
            int randomItem = Random.Range(0, 100);
            Debug.Log($"randomItem : {randomItem}");
            if (randomItem <= 70)
            {
                // Spawn Small Item
                Debug.Log("Spawn Small Item");
                InitItem(0);
            }
            else if (randomItem > 70 && randomItem <= 90)
            {
                // Spawn Medium Item
                Debug.Log("Spawn Medium Item");
                InitItem(1);
            }
            else
            {
                // Spawn Large Item
                Debug.Log("Spawn Large Item");
                InitItem(2);
            }
        }
    }
    [Server]
    private void InitItem(int itemIndex)
    {
        GameObject item = Instantiate(TankGameManager.Instance.PotionPrefabs, transform.position, Quaternion.identity);

        if (item.TryGetComponent(out ItemPickup pickup))
        {
            pickup.SetItemID(itemIndex); // Gán cho instance
        }

        NetworkServer.Spawn(item);
    }



}
