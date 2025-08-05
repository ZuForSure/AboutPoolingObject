using UnityEngine;
using Mirror;

public class Tank : NetworkBehaviour
{
    [SerializeField] private TankHeal tankHeal; public TankHeal TankHeal=>tankHeal;
    [SerializeField] private LookAtMouse lookAtMouse;
    [SerializeField] private TankMove tankMove;

    private void Awake()
    {
        tankHeal = GetComponentInChildren<TankHeal>();
        lookAtMouse = GetComponentInChildren<LookAtMouse>();
        tankMove = GetComponentInChildren<TankMove>();
        Debug.Log($"[Awake] isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}");
    }

    public override void OnStartClient()
    {
        Debug.Log($"[OnStartClient] isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}");

        if (!isLocalPlayer)
        {
            Debug.Log("Not local player ? Skip CmdInitTankHeal");
            return;
        }

        CmdInitTankHeal();
        CmdInitUiHeal();
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        if (tankHeal.IsDeath) return;
        tankMove.GetMoveDirection();
        lookAtMouse.AimTarget(lookAtMouse.GetTarget());

    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (tankHeal.IsDeath) return;
        tankMove.RbMove();
    }


    #region Commnad
    [Command]
    private void CmdInitTankHeal()
    {
        tankHeal.SetHealTank(TankGameManager.Instance.Heal);
    }
    [Command]
    private void CmdInitUiHeal()
    {
        RpcInitUiHeal();
    }    
    #endregion
    #region TargetRPC
    //[TargetRpc]
    //private void TargetSetTankHeal(NetworkConnection target)
    //{
    //    if (tankHeal != null)
    //    {
    //        tankHeal.SetHealTank(TankGameManager.Instance.Heal);
    //        Debug.Log($"healtank : {TankGameManager.Instance.Heal}");
    //    }
    //}
    #endregion
    #region ClientRPC
    [ClientRpc]
    private void RpcInitUiHeal()
    {
        UiManager.Instance.ShowUiHeal(true);
        UiManager.Instance.SetTextHeal(TankGameManager.Instance.Heal);
    }    
    #endregion

}
