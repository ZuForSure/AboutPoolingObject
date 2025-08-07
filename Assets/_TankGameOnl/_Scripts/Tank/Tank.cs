using Mirror;
using System;
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

    //public class SyncListInt : SyncList<int> { }
    public SyncList<int> inventory = new();
    public int maxSlots = 3;


    private void Awake()
    {
        tankHeal.Init(this);
        lookAtMouse = GetComponentInChildren<LookAtMouse>();
        tankMove.Init(GetComponent<Rigidbody2D>(), transform);
        Debug.Log($"[Awake] isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}");
       
        NetworkClient.RegisterHandler<SeverSendMessage>(OnServerSendMessage);
    }
    private void OnDestroy()
    {
        NetworkClient.UnregisterHandler<SeverSendMessage>();
    }




    void Start()
    {
        // Chỉ client mới cần update UI khi inventory thay đổi
        if (isClient)
        {
            inventory.Callback += OnInventoryChanged;
        }
        NetworkClient.Send(new ClientRequestSever());
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isLocalPlayer)
        {
            UiManager.Instance.OnButtonReadyClick -= OnReadyButtonClicked;
            UiManager.Instance.inventorySlot.ShowInventory(false);
            TargetRemoveAllItem();

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
        UiManager.Instance.inventorySlot.ShowInventory(true);

    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        RemoveAllItem();
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        if (isDeath) return;
        tankMove.GetInputMoveAndRotate();
        lookAtMouse.AimTarget(lookAtMouse.GetTarget());
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Gọi lệnh server để dùng item
            CmdUseItem(0); // Giả sử dùng item ở slot 0
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Gọi lệnh server để dùng item
            CmdUseItem(1); // Giả sử dùng item ở slot 1
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Gọi lệnh server để dùng item
            CmdUseItem(2); // Giả sử dùng item ở slot 2
        }

    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (isDeath) return;
        tankMove.RbMove();


    }



    #region Funtion

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

    // Gọi trên server khi muốn dùng item
    [Command]
    public void CmdUseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.Count)
            return;

        int itemID = inventory[slotIndex];

        // TODO: Áp dụng hiệu ứng của itemID cho player
        Debug.Log($"Server: Player dùng item {itemID}");
        switch (itemID)
        {
            case 0:
                healTank += 2; // Giả sử itemID 0 là item nhỏ, tăng 2 máu
                break;
            case 1:
                healTank += 4; // Giả sử itemID 1 là item trung bình, tăng 4 máu
                break;
            case 2:
                healTank += 6; // Giả sử itemID 2 là item lớn, tăng 6 máu
                break;
            default:
                Debug.LogWarning($"Unknown item ID: {itemID}");
                break;
        }

        inventory.RemoveAt(slotIndex); // Xóa item khỏi inventory
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

   
   

    #endregion

    #region TargetRPC

    [TargetRpc]
    private void TargetInitUiHeal()
    {
        UiManager.Instance.ShowUiHeal(true);
        UiManager.Instance.SetTextHeal(TankGameManager.Instance.Heal);
    }

    [TargetRpc]
    private void TargetIsGamePlaying(bool isGameplaying)
    {
        UiManager.Instance.ShowUiButtonReady(!isGameplaying, false);
    }

    [TargetRpc]
    private void TargetHideUI(bool isShow)
    {
        Debug.Log($"[TargetHideUI] isShow: {isShow}, isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}");
        UiManager.Instance.ShowUiButtonReady(!isShow);
    }
    //[TargetRpc]
    private void TargetRemoveAllItem()
    {
        Debug.Log($"[TargetRemoveAllItem] isLocalPlayer: {isLocalPlayer}, isClient: {isClient}, isServer: {isServer}, netId: {netId}");
        Debug.Log("All items removed from inventory.");
        UiManager.Instance.inventorySlot.ResetUiItem();
    }

    #endregion

    #region ClientRPC
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
    [Server]
    public bool AddItem(NetworkConnection conn, int itemID)
    {
        if (inventory.Count >= maxSlots)
            return false;

        inventory.Add(itemID); // Thêm item
        return true;
    }
    [Server]
    private void RemoveAllItem()
    {
        inventory.Clear(); // xử lý trên server

        //if (connectionToClient != null)
        //{
        //    TargetRemoveAllItem(); // Gửi RPC về client để cập nhật UI
        //}

        Debug.Log("All items removed from inventory (server-side).");
    }
    #endregion

    #region SyncList
    //Chạy ở client khi inventory thay đổi
    private void OnInventoryChanged(SyncList<int>.Operation op, int index, int oldItem, int newItem)
    {
        Debug.Log($"Inventory changed! Op: {op}, Slot: {index}");
        if (!isLocalPlayer) return;
        UiManager.Instance.inventorySlot.UpdateUI(this);
    }
    #endregion

    #region SyncVar
    private void OnChangeTankHeal(int oldValue, int newValue)
    {
        if (isLocalPlayer)
        {
            //Debug.Log($"healvalue {healTank}");
            UiManager.Instance.SetTextHeal(newValue);
        }

    }

    #endregion

    #region Message 

    private void OnServerSendMessage(SeverSendMessage message)
    {
        Debug.Log($"[OnServerSendMessage] severTime: {message.severTime}");
    }
    #endregion

}
