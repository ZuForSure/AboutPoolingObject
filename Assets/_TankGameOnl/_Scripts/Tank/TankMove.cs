using Mirror;
using Mirror.Examples.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TankMove 
{
    [SerializeField] protected Rigidbody2D tankRb;
    [SerializeField] protected float moveSpeed = 5f;

    [SyncVar] private Vector2 moveDirection;
    //private InputManager input;

    private void Awake()
    {
        
    }
    public void Init(Rigidbody2D rb2d)
    {
        this.tankRb = rb2d;
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
