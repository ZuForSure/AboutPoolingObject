using Mirror;
using Mirror.Examples.Benchmark;
using Mirror.Examples.Common.Controllers.Tank;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    [SerializeField] private Direction direction;
    [SerializeField] private BoatController tank;

    [SerializeField] private bool pressed;

    void Start()
    {
        if (tank == null) StartCoroutine(AssignLocalTankWhenReady());
    }

    IEnumerator AssignLocalTankWhenReady()
    {
        while (NetworkClient.localPlayer == null) yield return null;
        var ni = NetworkClient.localPlayer;
        tank = ni.GetComponent<BoatController>();
    }
   

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        tank.BoatMove.isDPad = true; // Đặt isDPad thành true khi nút được nhấn
        Apply(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        tank.BoatMove.isDPad = false; // Đặt isDPad thành false khi nút được nhả
        Apply(false);
    }

    // Khi trượt ngón tay/chuột ra khỏi nút thì cũng dừng
    public void OnPointerExit(PointerEventData eventData)
    {
        if (pressed)
        {
            pressed = false;
            tank.BoatMove.isDPad = false; // đặt isdpad thành false khi nút được nhả
            Apply(false);
        }
    }

    void Apply(bool state)
    {
        if (tank == null)
        {
            Debug.LogWarning("UIMoveButton: 'tank' hoặc 'TankMove' chưa được gán!");
            return;
        }
        switch (direction)
        {
            case Direction.Up: tank.BoatMove.moveUp = state; break;
            case Direction.Down: tank.BoatMove.moveDown = state; break;
            case Direction.Left: tank.BoatMove.moveLeft = state; break;
            case Direction.Right: tank.BoatMove.moveRight = state; break;
        }
    }
   
}
