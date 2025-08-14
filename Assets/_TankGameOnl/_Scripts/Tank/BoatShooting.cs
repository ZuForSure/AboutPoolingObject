using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoatShooting : NetworkBehaviour
{
    [SerializeField] protected bool canShoot = false;
    [SerializeField] protected GameObject bullet;
    [SerializeField] private BoatController boat;
    private void Update()
    {
        if (boat.IsDeath) return;
        if (!this.GetCanShoot()) return;
        this.SpawnBullet();
    }

    protected virtual bool GetCanShoot()
    {
        if (!isLocalPlayer) return false;

        this.canShoot = Input.GetKeyDown(KeyCode.Mouse0);
        return this.canShoot;
    }

    [Command]
    protected void SpawnBullet()
    {
        var newBullet = LeanPool.Spawn(this.bullet, transform.position, transform.rotation, null);
        NetworkServer.Spawn(newBullet);
    }
}
