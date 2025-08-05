using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankHeal : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnChangeTankHeal))]
    [SerializeField] private int healTank;

    [SyncVar]
    [SerializeField] private bool isDeath; 
    public bool IsDeath => isDeath;
    [SerializeField] private Tank tank;

    private void Awake()
    {
        tank = GetComponentInParent<Tank>();
    }


    public void SetHealTank(int heal)
    {
        Debug.Log($"heal : {heal}");
        healTank = heal;

    }
    public void ReduceHeal(int damage)
    {
        int oldValue = healTank;
        healTank -= damage;
        Debug.Log($"[SERVER] ReduceHeal: {oldValue} -> {healTank}");
        if ( healTank <= 0 )
        {
            healTank = 0;
            CmdPlayerDeath();
            //CmdPlayerHide();
            Debug.Log("tank Die");
            //PlayerDeath();
        }
    }
    private void OnChangeTankHeal(int oldValue,int newValue)
    {
        if(tank.isLocalPlayer)
        {
            Debug.Log($"healvalue {healTank}");
            UiManager.Instance.SetTextHeal(newValue);
        }    
        
    }    

    [Command]
    private void CmdPlayerDeath()
    {
        isDeath = true;
    }
    //[Command]
    //private void CmdPlayerHide()
    //{
    //    RpcPlayerHide();
    //}
    [ClientRpc]
    private void RpcPlayerHide()
    {
        tank.gameObject.SetActive(false);
    }    
   
}
