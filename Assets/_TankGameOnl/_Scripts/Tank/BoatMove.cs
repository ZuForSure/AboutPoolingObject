using Mirror;
using Mirror.Examples.Basic;
using Mirror.Examples.BilliardsPredicted;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

[Serializable]
public class BoatMove
{
    [SerializeField] protected Rigidbody2D boatRB;
    [SerializeField] private Transform transform;
    [SerializeField] protected float moveSpeed = 7f;
    [SerializeField] protected float rotateSpeed = 100f;
    [SerializeField] protected float horizontalInput;
    [SerializeField] protected float verticalInput;

    public bool moveUp, moveDown, moveLeft, moveRight;
    public bool isDPad;



    [SyncVar] private Vector2 moveDirection;
    //private InputManager input;

    public void Init(Rigidbody2D rb2d, Transform transform)
    {
        this.boatRB = rb2d;
        this.transform = transform;

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

        //boatRB.MoveRotation(boatRB.rotation + rotationAmount);
        transform.Rotate(Vector3.forward * rotationAmount);
        boatRB.velocity = moveDirection * moveSpeed;
    }

    public void RbMoveWithInput()
    {
        float forward = 0f;
        if (moveUp) forward += 1f;
        if (moveDown) forward -= 1f;

        float turn = 0f;
        if (moveLeft) turn += 1f;
        if (moveRight) turn -= 1f;

        float deltaRot = -turn * rotateSpeed * Time.fixedDeltaTime;
        transform.Rotate(Vector3.forward * deltaRot);

        Vector2 vel = (Vector2)boatRB.transform.up * (forward * moveSpeed);
        boatRB.velocity = vel;

    }


}
