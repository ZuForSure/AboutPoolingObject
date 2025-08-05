using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankHeal : MonoBehaviour
{
    [SerializeField] private int healTank;


    public void SetHealTank(int heal)
    {
        Debug.Log($"heal : {heal}");
        healTank = heal;

    }
}
