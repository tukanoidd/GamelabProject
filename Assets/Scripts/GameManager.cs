using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    public bool gamePaused = false;

    private void Awake()
    {
        current = this;
    }
}
