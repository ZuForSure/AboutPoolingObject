using PinePie.SimpleJoystick;
using System;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

[Serializable]
public class BoatMove
{
    [SerializeField] private JoystickController joystick;

    public Rigidbody2D boatRB;
    [SerializeField] private Transform transform;
    [SerializeField] protected float moveSpeed = 7f;
    [SerializeField] protected float rotateSpeed = 100f;
    [SerializeField] protected float horizontalInput;
    [SerializeField] protected float verticalInput;

    public bool moveUp, moveDown, moveLeft, moveRight;
    public bool isDPad;

    //[SyncVar] private Vector2 moveDirection;
    //private InputManager input;

    public void Init(Rigidbody2D rb2d, Transform transform, JoystickController joy)
    {
        this.boatRB = rb2d;
        this.transform = transform;
        this.joystick = joy;
    }

    public void GetInputMoveAndRotate()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        if (Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f))
        {
            horizontalInput = joystick.InputDirection.x;
            verticalInput = joystick.InputDirection.y;
        }
    }

    public void RbMove()
    {
        //float rotationAmount = -horizontalInput * rotateSpeed * Time.deltaTime;
        //Vector2 moveDirection = boatRB.transform.up * verticalInput;

        //transform.Rotate(Vector3.forward * rotationAmount);
        //boatRB.velocity = moveDirection * moveSpeed;

        //Debug.Log($"Horizontal: {horizontalInput}, Vertical: {verticalInput}");

        if (Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f))
        {
            boatRB.velocity = Vector2.zero;
            return;
        }

        Vector2 inputDir = new Vector2(horizontalInput, verticalInput).normalized;

        float targetAngle = Mathf.Atan2(inputDir.y, inputDir.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotateSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);

        boatRB.velocity = transform.up * moveSpeed;
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
