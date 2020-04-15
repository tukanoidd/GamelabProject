using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEventSystem : MonoBehaviour
{
    public static LevelEventSystem current;
    
    public event Action<int> onBlockClicked;

    private void Awake()
    {
        current = this;
    }

    public void OnBlockClicked(int id)
    {
        onBlockClicked?.Invoke(id);
    }
}
