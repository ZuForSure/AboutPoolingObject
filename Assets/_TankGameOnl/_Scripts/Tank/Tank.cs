using Mirror;
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
    [SyncVar]
    [SerializeField] private bool isReady;
    public bool IsReady => isReady;

    private void Awake()
    {
        tankHeal.Init(this);
        lookAtMouse = GetComponentInChildren<LookAtMouse>();
        tankMove.Init(GetComponent<Rigidbody2D>());
        Debug.Log($"[Awake] isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}");
    }

    private void Start()
    {
        UiManager.Instance.OnButtonReadyClick += () =>
        {
            CmdSetReady(true);
        };
    }

    public override void OnStartClient()
    {
        Debug.Log($"[OnStartClient] isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}, author : {authority}");

        if (!isLocalPlayer)
        {
            Debug.Log("Not local player ? Skip CmdInitTankHeal");
            return;
        }

        CmdInitTankHeal();
        CmdInitUiHeal();
        UiManager.Instance.ShowUiButtonReady(true);
       
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

    #region Funtion
    private void TargetHideUI(bool isShow)
    {
        Debug.Log($"[TargetHideUI] isShow: {isShow}, isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}");
        UiManager.Instance.ShowUiButtonReady(!isShow);
    }
    public void SetIsReady(bool isReady)
    {
        this.isReady = isReady;  
    }    
    #endregion

    #region Commnad

    [Command]
    public void CmdSetReady(bool value)
    {
        Debug.Log($"[CmdSetReady] isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}");
        isReady = value;
        //TargetActionOnReadyGame();

        TargetHideUI(TankGameManager.Instance.CheckAllPlayersReady());

    }

    [Command]
    private void CmdInitTankHeal()
    {
        SetHealTank(TankGameManager.Instance.Heal);
    }

    [Command]
    private void CmdInitUiHeal()
    {
        TargetInitUiHeal();
    }
    #endregion

    #region TargetRPC
    [TargetRpc]
    private void TargetInitUiHeal()
    {
        UiManager.Instance.ShowUiHeal(true);
        UiManager.Instance.SetTextHeal(TankGameManager.Instance.Heal);
    }
    [TargetRpc]
    //private void TargetActionOnReadyGame()
    //{
    //    UiManager.Instance.OnReadyGame?.Invoke(isReady);
    //}
  
    #endregion

    #region ClientRPC

    #endregion

    #region Server
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
        Destroy(gameObject, 0.5f);
    }
    #endregion
}
