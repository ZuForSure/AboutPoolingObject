using Mirror;
using Mirror.BouncyCastle.Pkcs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerMove : NetworkBehaviour
{
    public class SyncItemList : SyncList<string> { }
    public SyncItemList inventory = new();
    public enum BulletType
    {
        Bullet_Default,
        Bullet_Heal,
        Bullet_Mana,
    }

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject drone;
    [SerializeField] private GameObject explosion;
    [SyncVar]
    public string playerName;
    [SyncVar(hook = nameof(OnChangeMoveSpeed))]
    public float moveSpeed = 5f;
    [SyncVar]
    [SerializeField] private int count;
    private bool isSend;
    [SerializeField] private bool isDroneFly;

    [SyncVar(hook = nameof(OnChangeColor))] public Color playerColor;
    [SerializeField] private BulletType bulletType;
    [SerializeField] private GameObject[] bulletArray;
    public float radius = 2f;     // Khoảng cách từ object đến player
    public float rotateSpeed = 100f; // Độ nhanh của vòng quay

    private const string PATH_PREFABS_BULLET = "Custom Network Prefabs/Bullet Prefabs";

    public override void OnStartServer()
    {
        if (connectionToClient != null)
        {
            int connID = connectionToClient.connectionId;
            playerName = $"Player : {connID}";
        }
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        inventory.Callback += OnItemChanged;
    }
    private void Start()
    {
        object[] bulletResources = Resources.LoadAll(PATH_PREFABS_BULLET);
        GameObject[] bulletArray = new GameObject[bulletResources.Length];
        for (int i = 0; i < bulletResources.Length; i++)
        {
            bulletArray[i] = bulletResources[i] as GameObject;
        }
        this.bulletArray = bulletArray;
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        float moveX = Input.GetAxis("Horizontal"); // A/D hoặc phím trái/phải
        float moveY = Input.GetAxis("Vertical");   // W/S hoặc phím lên/xuống

        Vector3 moveDir = new Vector3(moveX, moveY, 0); // Di chuyển trên mặt phẳng XZ

        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Player : {playerName}");
            CmdDebugName();
            CmdInit();
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            CmdChangeSpeed();
            CmdDebugName();
            count++;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CmdChangeColor();
            CmdDebugName();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            CmdChangeBullet();
            CmdDebugName();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            isDroneFly = !isDroneFly;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            CmdInitDrone();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            CmdAddItem("Posion");
        }
        if (count >= 3 && !isSend)
        {
            CmdEnoughCount();
            isSend = true;
        }
        if (isDroneFly && drone != null)
        {
            DroneRotation();
        }





    }
    private void OnItemChanged(SyncList<string>.Operation op , int index , string oldItem,string newItem)
    {
        Debug.Log($"Client thấy thay đổi: {op} tại index {index}");
    }
    private void OnChangeColor(Color oldColor, Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;
    }
    private void OnChangeMoveSpeed(float oldSpeed, float newSpeed)
    {
        if (isLocalPlayer)
            Debug.Log($"Thay đổi speed từ {oldSpeed} -> {newSpeed}");
    }
    private void DroneRotation()
    {
        Debug.Log("test");
        if (drone == null) return;

        // Đặt lại vị trí ban đầu nếu chưa từng đặt
        if (Vector3.Distance(drone.transform.position, transform.position) < 0.01f)
        {
            Vector3 offset = new Vector3(radius, 0, 0); // hoặc new Vector3(0, radius, 0)
            drone.transform.position = transform.position + offset;
        }

        drone.transform.RotateAround(transform.position, Vector3.forward, rotateSpeed * Time.deltaTime);
    }
    #region Command
    [Command]
    public void CmdAddItem(string newItem)
    {
        // Bước 4: Server thêm vào danh sách
        inventory.Add(newItem);
    }
    [Command]
    private void CmdChangeColor()
    {
        playerColor = Random.ColorHSV();
        GetComponent<SpriteRenderer>().color = playerColor;
    }

    [Command]
    private void CmdEnoughCount()
    {
        RpcOpen();
    }
    [Command]
    private void CmdInit() // gửi lên sever và gọi trên sever để dồng bộ
    {
        RpcInitBullet();
        RpcInitExplosion();
    }
    [Command]
    void CmdDebugName()
    {
        Debug.Log($"[SERVER] Tên client gửi lệnh là: {playerName}");
    }
    [Command]
    private void CmdChangeSpeed()
    {

        moveSpeed = 10;
    }
    [Command]
    private void CmdChangeBullet()
    {
        TargetChangeBullet();
    }
    [Command]
    private void CmdInitDrone()
    {
        GameObject droneClone = Instantiate(drone);
        NetworkServer.Spawn(droneClone, connectionToClient);
        // Gửi về client: “đây là drone của bạn”
        TargetSetDrone(connectionToClient, droneClone);
    }
    [Command]
    private void CmdDroneMove()
    {
        //TargetSetDrone();
    }
    #endregion

    #region ClientRPC
    [ClientRpc]
    private void RpcInitBullet()
    {
        GameObject bullet = Instantiate(this.bullet);
        bullet.transform.position = this.transform.position;
        //NetworkServer.Spawn(bullet);
    }

    [ClientRpc]
    private void RpcInitExplosion()
    {
        GameObject explosion = Instantiate(this.explosion);
        Vector3 pos = new(transform.position.x + 2, transform.position.y, transform.position.z);
        explosion.transform.position = pos;
    }
    #endregion

    #region TargetRpc
    [TargetRpc]
    private void RpcOpen()
    {
        Debug.Log("Mở Kỹ năng");
    }
    [TargetRpc]
    private void TargetChangeBullet()
    {
        switch (bulletType)
        {
            case BulletType.Bullet_Heal:
                bullet = bulletArray[1];
                break;
            case BulletType.Bullet_Mana:
                bullet = bulletArray[2];
                break;
            case BulletType.Bullet_Default:
                bullet = bulletArray[0];
                break;
            default:

                break;

        }
    }
    [TargetRpc]
    private void TargetSetDrone(NetworkConnection conn, GameObject droneInstance)
    {
        drone = droneInstance;

        // Đặt drone lệch khỏi player ban đầu
        Vector3 offset = new Vector3(radius, 0, 0);
        drone.transform.position = transform.position + offset;

        isDroneFly = true;
    }


    #endregion
    //[Client]
    //void UpdateHUD()
    //{
    //    // chỉ chạy ở Client
    //    Debug.Log("Updating HUD on client");
    //}
    //[ClientCallback]
    //void UpdateCallback()
    //{
    //    // chỉ chạy ở Client
    //    Debug.Log("Updating Callback on client");
    //}






}
