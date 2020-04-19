using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    public static GameManager current;
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    [NonSerialized] public DeviceType deviceType;
    [NonSerialized] public bool gamePaused = false;
    
    [NonSerialized] public Player player;
    [NonSerialized] public TurnAroundCamera mainCamera;
    [NonSerialized] public PathFinder pathFinder;

    [NonSerialized] public bool playerLockedMovement = false;
    [NonSerialized] public bool cameraLockedMovement = false;
    //--------Private and Public Invisible In Inspector--------\\
    
    //--------Static Behavior--------\\
    public static IEnumerator Countdown(float duration, Action funcToExecute)
    {
        float normalizedTime = 0;
        while(normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        funcToExecute();
    }
    //--------Static Behavior--------\\
    
    private void Awake()
    {
        current = this;

        deviceType = SystemInfo.deviceType;

        player = FindObjectOfType<Player>();
        mainCamera = FindObjectOfType<TurnAroundCamera>();
        pathFinder = FindObjectOfType<PathFinder>();
    }
}
