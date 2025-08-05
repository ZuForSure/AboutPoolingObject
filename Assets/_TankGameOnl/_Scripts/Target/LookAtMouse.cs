using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : LookAtTarget
{
 
    
    public override Vector3 GetTarget()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        this.target = mouseWorld;
        return target;
    }
}
