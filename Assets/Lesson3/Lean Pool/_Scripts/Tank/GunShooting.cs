using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShooting : MonoBehaviour
{
    [SerializeField] protected bool canShoot = false;
    [SerializeField] protected GameObject bullet;

    private void Update()
    {
        if (!this.GetCanShoot()) return;
        this.SpawnBullet();
    }

    protected virtual bool GetCanShoot()
    {
        //this.canShoot = InputManager.Instance.ShootInput;
        return this.canShoot;
    }

    protected virtual void SpawnBullet()
    {
        LeanPool.Spawn(this.bullet, transform.position, transform.rotation, null);
    }
}
