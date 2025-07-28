using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LookAtTarget : MonoBehaviour
{
    [Header("Look At Targets")]
    [SerializeField] protected Vector3 target;

    protected void AimTarget(Vector3 target)
    {
        Vector2 diff = target - this.transform.position;
        diff = diff.normalized;
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);
    }

    protected abstract void GetTarget();
}
