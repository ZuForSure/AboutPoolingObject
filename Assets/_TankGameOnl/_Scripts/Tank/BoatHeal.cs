using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoatHeal
{
    private BoatController tank;

    public void Init(BoatController tank)
    {
        this.tank = tank;
    }

    //public void SetHealTank(int defaultHeal)
    //{
    //    tank.SetHealTank(defaultHeal); // Gọi sang Tank để set SyncVar
    //}

    public void ReduceHeal(int damage)
    {
        Debug.Log($"ReduceHeal - Sever : {NetworkServer.active} | Client : {NetworkClient.active}");
        if (!tank.isServer) return; // Chỉ cho server xử lý logic này
        int newHeal = tank.HealTank - damage;

        if (newHeal <= 0)
        {
            newHeal = 0;
            tank.SetDeath(true);
            tank.SetIsReady(false); // Đặt trạng thái không sẵn sàng khi chết

            EnemySpawner.Instance.CheckCanContinueSpawn();
        }

        tank.SetHealTank(newHeal);
      
    }

}
