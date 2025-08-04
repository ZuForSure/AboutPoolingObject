using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using kcp2k;

[RequireComponent(typeof(KcpTransport))]
public class CusNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("OnStartServer");
    }
    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("OnStartHost");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("OnStartClient");
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("OnStopClient");
    }
    public override void OnStopHost()
    {
        base.OnStopHost();
        Debug.Log("OnStopHost");
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("OnStopServer");
    }
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("OnClientConnect");
    }
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        Debug.Log("OnClientDisconnect");
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab);
        //player.GetComponent<PlayerMove>().playerName = $"Client {conn.connectionId}";
        NetworkServer.AddPlayerForConnection(conn, player);
    }
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("OnServerConnect");
       
       
    }
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        Debug.Log("OnServerReady");
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("OnServerDisconnect");
    }
    
   
}
