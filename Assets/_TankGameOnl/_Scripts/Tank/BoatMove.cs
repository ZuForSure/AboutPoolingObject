using Mirror;
using Mirror.Examples.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[Serializable]
public class BoatMove 
{
    [SerializeField] protected Rigidbody2D boatRB;
    [SerializeField] protected float moveSpeed = 7f;
    [SerializeField] protected float rotateSpeed = 50f;
    [SerializeField] protected float horizontalInput;
    [SerializeField] protected float verticalInput;

    [SyncVar] private Vector2 moveDirection;
    //private InputManager input;

    public void Init(Rigidbody2D rb2d)
    {
        this.boatRB = rb2d;
    }

    public void GetInputMoveAndRotate()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }

    public void RbMove()
    {
        float rotationAmount = -horizontalInput * rotateSpeed * Time.deltaTime;
        Vector2 moveDirection = boatRB.transform.up * verticalInput;

        boatRB.MoveRotation(boatRB.rotation + rotationAmount);
        boatRB.velocity = moveDirection * moveSpeed;
    }    

}
