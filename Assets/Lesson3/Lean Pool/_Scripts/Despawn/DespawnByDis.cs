//using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnByDis : MonoBehaviour
{
    [Header("Despawn By Distance")]
    [SerializeField] protected float maxDis = 20f;
    [SerializeField] protected float dis;
    [SerializeField] protected bool canDesepawn = false;

    protected virtual void Update()
    {
        this.CheckDistance();
    }

    protected virtual void CheckDistance()
    {
        this.dis = Vector2.Distance(Camera.main.transform.position, transform.parent.position);
        if (this.dis >= this.maxDis) this.canDesepawn = true;
    }

    public virtual bool GetCanDespawn()
    {
        return this.canDesepawn;
    }
}
