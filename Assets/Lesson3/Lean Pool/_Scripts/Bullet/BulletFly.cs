using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFly : MonoBehaviour
{
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected Vector2 flyDirec = Vector2.right;

    void FixedUpdate()
    {
        this.Fly();
    }

    protected virtual void Fly()
    {
        transform.parent.Translate(this.flyDirec * this.speed * Time.fixedDeltaTime);
    }
}
