using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour 
{
    [SerializeField] protected EnemyFollow enemyFollow;

    private void FixedUpdate()
    {
        if(enemyFollow.player == null) return;

        this.AimTarget(enemyFollow.player.transform.position);
    }

    public void AimTarget(Vector3 target)
    {
        Vector2 diff = target - this.transform.position;
        diff = diff.normalized;
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90f);
    }
}
