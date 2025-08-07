using Mirror;
using UnityEngine;

public class Tank : NetworkBehaviour
{
    [SerializeField] private TankHeal tankHeal; public TankHeal TankHeal => tankHeal;
    [SerializeField] private LookAtMouse lookAtMouse;
    [SerializeField] private BoatMove tankMove;
    [SyncVar(hook = nameof(OnChangeTankHeal))]
    [SerializeField] private int healTank;
    public int HealTank => healTank;
    [SyncVar]
    [SerializeField] private bool isDeath;
    public bool IsDeath => isDeath;

    [SyncVar] private bool isReady = false;
    public bool IsReady { get => isReady; set => isReady = value; }


    private void Awake()
    {
        tankHeal.Init(this);
        lookAtMouse = GetComponentInChildren<LookAtMouse>();
        tankMove.Init(GetComponent<Rigidbody2D>());
        Debug.Log($"[Awake] isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isLocalPlayer)
        {
            UiManager.Instance.OnButtonReadyClick -= OnReadyButtonClicked;
        }
    }

    public override void OnStartClient()
    {
        Debug.Log($"[OnStartClient] isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}, author : {authority}");

        if (!isLocalPlayer)
        {
            Debug.Log("Not local player ? Skip CmdInitTankHeal");
            return;
        }
        UiManager.Instance.OnButtonReadyClick += OnReadyButtonClicked;

        CmdInitTankHeal();
        CmdInitUiHeal();
        CmdIsGamePlaying();
    }


    private void Update()
    {
        if (!isLocalPlayer) return;
        if (isDeath) return;
        tankMove.GetInputMoveAndRotate();
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
    [TargetRpc]
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

    public void OnReadyButtonClicked()
    {
        CmdSetReady(true);
    }

    [Command]
    public void CmdSetReady(bool value)
    {
        Debug.Log($"[CmdSetReady] isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}");
        Debug.Log("CmdReady: " + value);
        isReady = value;
        //TargetActionOnReadyGame();

        bool isAllPleyrReady = TankGameManager.Instance.CheckAllPlayersReady();

        TargetHideUI(isAllPleyrReady);


        if (isAllPleyrReady)
        {
            RpcAllPlayerReady();
            RpcShowSliderExp();
        }
    }

    [Command]
    private void CmdInitTankHeal()
    {
        Debug.Log($"CmdInitTankHeal - Sever : {isServer} - Client : {isClient}");
        SetHealTank(TankGameManager.Instance.Heal);
    }

    [Command]
    private void CmdInitUiHeal()
    {
        TargetInitUiHeal();
    }

    [Command]
    private void CmdIsGamePlaying()
    {
        Debug.Log("Is playing: " + TankGameManager.Instance.IsPlaying);
        TargetIsGamePlaying(TankGameManager.Instance.IsPlaying);
    }


    [ClientRpc]
    void RpcAllPlayerReady()
    {
        UiManager.Instance.ShowUiButtonReady(false);
    }
    [ClientRpc]
    void RpcShowSliderExp()
    {
        UiManager.Instance.ShowUiSlider(true);
    }


    [TargetRpc]
    private void TargetIsGamePlaying(bool isGameplaying)
    {
        UiManager.Instance.ShowUiButtonReady(!isGameplaying, false);
    }
    #endregion

    #region TargetRPC
    [TargetRpc]
    private void TargetInitUiHeal()
    {
        UiManager.Instance.ShowUiHeal(true);
        UiManager.Instance.SetTextHeal(TankGameManager.Instance.Heal);
    }
    //[TargetRpc]
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
        Debug.Log($"SetHealTank - Sever : {isServer} - Client : {isClient}");
        Debug.Log($"SetHealTank - healTank : {healTank} - Value : {value}");
        healTank = value;
        // Hook sẽ tự gọi OnChangeTankHeal
    }

    [Server]
    public void SetDeath(bool value)
    {
        isDeath = value;

        TankNetworkManager.Instance.Players.Remove(gameObject);
        Destroy(gameObject, 0.5f);
    }
    #endregion
}
