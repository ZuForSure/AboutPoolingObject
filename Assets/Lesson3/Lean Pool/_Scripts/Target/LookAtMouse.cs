using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : LookAtTarget
{
    private void Update()
    {
        this.GetTarget();
    }

    private void LateUpdate()
    {
        this.AimTarget(this.target);
    }

    protected override void GetTarget()
    {
        this.target = InputManager.Instance.MouseWorldPoint;
    }
}
