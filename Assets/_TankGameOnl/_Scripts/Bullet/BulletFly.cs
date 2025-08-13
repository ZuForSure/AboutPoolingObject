using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Transform = UnityEngine.Transform;

[Serializable]
public class BulletFly
{
    [SerializeField] private Transform transform;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Vector2 flyDirec = Vector2.right;

    public void Init(Transform transform)
    {
        this.transform = transform;
    }

    public void Fly()
    {
        transform.parent.Translate(this.flyDirec * this.speed * Time.fixedDeltaTime);
    }
}
