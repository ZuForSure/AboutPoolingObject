using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankGameManager : ZuSingleton<TankGameManager> 
{
    [SerializeField] private int heal; public int Heal=> heal;
}
