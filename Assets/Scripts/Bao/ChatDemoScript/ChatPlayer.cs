using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatPlayer : NetworkBehaviour 
{
    [Command]
    public void CmdSendMessage(string msg)
    {
        string fullMsg = $"{connectionToClient.connectionId} : {msg}";

    }
    [TargetRpc]
    public void TargetReceiveMessage(NetworkConnection target , string msg)
    {
        
    } 
        



}

