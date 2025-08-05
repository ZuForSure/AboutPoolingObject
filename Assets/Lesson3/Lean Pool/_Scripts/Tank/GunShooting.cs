using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GunShooting : NetworkBehaviour
{
    [SerializeField] protected bool canShoot = false;
    [SerializeField] protected GameObject bullet;
    [SerializeField] private Tank tank;
    private void Update()
    {
        if (tank.TankHeal.IsDeath) return;
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
    protected virtual void SpawnBullet()
    {
        var newBullet = LeanPool.Spawn(this.bullet, transform.position, transform.rotation, null);
        NetworkServer.Spawn(newBullet);
    }
}
