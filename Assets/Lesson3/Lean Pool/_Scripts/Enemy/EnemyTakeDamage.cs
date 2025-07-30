using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeDamage : MonoBehaviour
{
    public int maxHp = 10;
    public int currentHP = 10;

    private void OnEnable()
    {
        this.currentHP = this.maxHp;
    }

    public virtual void DeductHP(int amount)
    {
        if (this.currentHP == 0) this.DeSpawnEnemey();

        this.currentHP -= amount;
    }

    protected virtual void DeSpawnEnemey()
    {
        LeanPool.Despawn(transform.parent);
    }
}
