using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMove : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D tankRb;
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected Vector3 moveDirection;

    private void Awake()
    {
        this.tankRb = GetComponentInParent<Rigidbody2D>();
    }

    void Update()
    {
        this.GetMoveDirection();
    }

    private void FixedUpdate()
    {
        this.Moving();
    }

    protected virtual void GetMoveDirection()
    {
        float move_x = InputManager.Instance.HoriInput;
        float move_y = InputManager.Instance.VertiInput;
        this.moveDirection = new Vector2(move_x, move_y).normalized;
    }

    protected virtual void Moving()
    {
        //this.tankRb.MovePosition(newPos * this.moveSpeed * Time.fixedDeltaTime);
        this.tankRb.velocity = this.moveDirection * this.moveSpeed;
    }
}
