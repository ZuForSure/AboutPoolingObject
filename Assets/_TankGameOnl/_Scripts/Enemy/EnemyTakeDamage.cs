using Lean.Pool;
using Mirror;
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
        if (this.currentHP == 0) this.DeSpawnEnemey();

        this.currentHP -= amount;
    }

    [Server]
    protected virtual void DeSpawnEnemey()
    {
        NetworkServer.UnSpawn(transform.parent.gameObject); 
        LeanPool.Despawn(transform.parent);
        LevelManager.Instance.AddExp(exp);
    }

}
