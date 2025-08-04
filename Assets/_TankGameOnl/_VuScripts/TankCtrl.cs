using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class TankCtrl : NetworkBehaviour
{
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        var vCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vCam != null)
        {
            vCam.Follow = transform;
            vCam.LookAt = transform; 
        }
    }
}
