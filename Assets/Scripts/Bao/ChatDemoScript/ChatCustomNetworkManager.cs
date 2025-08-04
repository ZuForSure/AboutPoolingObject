using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChatCustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player); // gán player thành object ch? ?ích cho client
    }
}
