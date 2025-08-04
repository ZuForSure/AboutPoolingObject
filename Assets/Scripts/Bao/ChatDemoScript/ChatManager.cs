using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class ChatManager : NetworkBehaviour
{
    public static ChatManager instance;
    private List<ChatPlayer> players = new();

    private void Awake()
    {
        instance = this;
    }

    public void Register(ChatPlayer player)
    {
        players.Add(player);
    }    
    public void SendToAllClients(string msg)
    {
        foreach (ChatPlayer player in players)
        {
            player.TargetReceiveMessage(player.connectionToClient, msg);
        } 
            
    }    
}
