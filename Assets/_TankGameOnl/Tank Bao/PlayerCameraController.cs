using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerCamera;

    public override void OnStartLocalPlayer()
    {
        playerCamera.gameObject.SetActive(true);
        playerCamera.tag = "MainCamera"; 
    }

    public override void OnStopLocalPlayer()
    {
        playerCamera.tag = "Untagged";
        playerCamera.gameObject.SetActive(false);
    }
}
