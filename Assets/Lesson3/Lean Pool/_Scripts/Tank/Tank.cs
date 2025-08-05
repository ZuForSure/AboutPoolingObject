using Mirror;
using Mirror.Examples.Tanks;
using UnityEngine;

public class Tank : NetworkBehaviour
{
    [SerializeField] private TankHeal tankHeal; public TankHeal TankHeal => tankHeal;
    [SerializeField] private LookAtMouse lookAtMouse;
    [SerializeField] private TankMove tankMove;
    [SyncVar(hook = nameof(OnChangeTankHeal))]
    [SerializeField] private int healTank;
    public int HealTank => healTank;
    [SyncVar]
    [SerializeField] private bool isDeath;
    public bool IsDeath => isDeath;

    private void Awake()
    {
        tankHeal.Init(this);
        lookAtMouse = GetComponentInChildren<LookAtMouse>();
        tankMove.Init(GetComponent<Rigidbody2D>());
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
        if (isDeath) return;
        tankMove.GetMoveDirection();
        lookAtMouse.AimTarget(lookAtMouse.GetTarget());

    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (isDeath) return;
        tankMove.RbMove();
    }

    private void OnChangeTankHeal(int oldValue, int newValue)
    {
        if (isLocalPlayer)
        {
            //Debug.Log($"healvalue {healTank}");
            UiManager.Instance.SetTextHeal(newValue);
        }

    }


    #region Commnad
    [Command]
    private void CmdInitTankHeal()
    {
        SetHealTank(TankGameManager.Instance.Heal);
    }
    [Command]
    private void CmdInitUiHeal()
    {
        RpcInitUiHeal();
    }
    #endregion
    #region TargetRPC

    #endregion
    #region ClientRPC
    [ClientRpc]
    private void RpcInitUiHeal()
    {
        UiManager.Instance.ShowUiHeal(true);
        UiManager.Instance.SetTextHeal(TankGameManager.Instance.Heal);
    }
    [ClientRpc]
    private void RpcSetTankVisibility(bool isVisible)
    {
        // Ẩn Renderer
        foreach (var r in GetComponentsInChildren<SpriteRenderer>())

            if (r != null)
            {
                r.enabled = isVisible;
            }
            else
            {
                Debug.LogWarning("No SpriteRenderer found in children.");
            }

        if (TryGetComponent<Collider2D>(out var collider2D))
        {
            // Ẩn Collider
            collider2D.enabled = isVisible;
        }

    }
    #endregion
    [Server]
    public void SetHealTank(int value)
    {
        healTank = value;
        // Hook sẽ tự gọi OnChangeTankHeal
    }
    [Server]
    public void SetDeath(bool value)
    {
        isDeath = value;
        RpcSetTankVisibility(false);
    }
}
