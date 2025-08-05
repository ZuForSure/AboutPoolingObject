using Mirror;
using Mirror.Examples.Basic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMove : NetworkBehaviour
{
    [SerializeField] protected Rigidbody2D tankRb;
    [SerializeField] protected float moveSpeed = 5f;

    [SyncVar] private Vector2 moveDirection;
    //private InputManager input;

    private void Awake()
    {
        this.tankRb = GetComponentInParent<Rigidbody2D>();
    }

   
    public void GetMoveDirection()
    {
        moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }   
    public void RbMove()
    {
        tankRb.velocity = moveDirection * moveSpeed;
    }    

}
