using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : ZuSingleton<InputManager>
{
    [SerializeField] protected bool shootInput = false;
    [SerializeField] protected Vector3 mouseWordPos;
    [SerializeField] protected float horizontal;
    [SerializeField] protected float vertical;
    public bool ShootInput => shootInput;
    public Vector3 MouseWorldPoint => mouseWordPos;
    public float HoriInput => horizontal;
    public float VertiInput => vertical;

    private void Update()
    {
      
        this.GetShootInput();
        this.GetMousePosInput();
        this.GetAxisInput();
    }

    protected virtual void GetShootInput()
    {
        this.shootInput = Input.GetKeyDown(KeyCode.Mouse0);
    }

    protected virtual void GetMousePosInput()
    {
        this.mouseWordPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.mouseWordPos.z = 0;
    }

    protected virtual void GetAxisInput()
    {
        this.horizontal = Input.GetAxisRaw("Horizontal");
        this.vertical = Input.GetAxisRaw("Vertical");
    }
}
