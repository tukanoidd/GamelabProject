using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    public static GameManager current;

    public DeviceType deviceType;
    public bool gamePaused = false;
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    private Player _player;

    [NonSerialized] public bool playerLockedMovement = false;
    [NonSerialized] public bool cameraLockedMovement = false;
    //--------Private and Public Invisible In Inspector--------\\
    
    private void Awake()
    {
        current = this;

        deviceType = SystemInfo.deviceType;

        _player = FindObjectOfType<Player>();
    }
}
