using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDespawn : DespawnByDis
{
    private void OnEnable()
    {
        this.canDesepawn = false;
    }

    protected override void CheckDistance()
    {
        base.CheckDistance();
        this.Despawning();
    }

    protected virtual void Despawning()
    {
        if (!this.canDesepawn) return;
        LeanPool.Despawn(transform.parent);
    }
}
