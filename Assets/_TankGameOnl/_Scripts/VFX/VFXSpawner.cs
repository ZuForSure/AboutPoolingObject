using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Lean.Pool;

public class VFXSpawner : ZuSingleton<VFXSpawner>
{
    public List<VFXController> VFXList;

    [Server]
    public void Spawning(VFXType type,Vector3 pos, Quaternion rot)
    {
        GameObject vfx = LeanPool.Spawn(this.GetVFXFromList(type), pos, rot);
        NetworkServer.Spawn(vfx);
    }

    protected GameObject GetVFXFromList(VFXType type)
    {
        foreach (VFXController vfx in VFXList)
        {
            if (vfx.type != type) continue;
            return vfx.gameObject;
        }

        return null;
    }
}
