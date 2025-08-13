using Mirror;
using PinePie.SimpleJoystick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    [SerializeField] private BulletFly bulletFly;
    [SerializeField] private BulletDespawn bulletDespawn;

    private void OnEnable()
    {
        if (!isLocalPlayer) return;
        bulletFly.Init(transform);
        bulletDespawn.Init(transform);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        bulletFly.Fly();
    }

    private void Update()
    {
        if (!isClient) return;
        bulletDespawn.DespawnByDistance();
    }
}
