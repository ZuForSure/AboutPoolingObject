using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BulletFly : NetworkBehaviour
{
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected Vector2 flyDirec = Vector2.right;

    void FixedUpdate() => Fly();

    protected void Fly()
    {
        if (!isServer) return;
        transform.parent.Translate(flyDirec * this.speed * Time.fixedDeltaTime);
    }
}
