using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemtFollow : MonoBehaviour
{
    public Transform player;     
    public float speed = 4f;     

    void Update()
    {
        if (player == null) return;
        Vector3 direction = (player.position - transform.parent.position).normalized;
        transform.parent.position += direction * speed * Time.deltaTime;
    }
}
