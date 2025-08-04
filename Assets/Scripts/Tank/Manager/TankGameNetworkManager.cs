using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankGameNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("Test");
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn,player);

    }
}
