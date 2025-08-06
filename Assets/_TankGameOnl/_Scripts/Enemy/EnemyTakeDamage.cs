using Lean.Pool;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTakeDamage : NetworkBehaviour
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
        if (this.currentHP == 0) this.DeSpawnEnemey();

        this.currentHP -= amount;
    }

    [Server]
    protected virtual void DeSpawnEnemey()
    {
        NetworkServer.UnSpawn(transform.parent.gameObject); 
        LeanPool.Despawn(transform.parent);
        LevelManager.Instance.AddExp(exp);
        Debug.Log($"Sever : {isServer} - Client : {isClient}");
        Debug.Log($"DeSpawnEnemey - Exp : {exp} - CurrentExp : {LevelManager.Instance.CurrentExp}");
    }
    //[TargetRpc]
    //private void TargetAddExp()
    //{
    //    Debug.Log($"Sever : {isServer} - Client : {isClient}");
    //    Debug.Log($"TargetAddExp - Exp : {exp} - CurrentExp : {LevelManager.Instance.CurrentExp}");
    //    LevelManager.Instance.AddExp(exp);
    //}

}
