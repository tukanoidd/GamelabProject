using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEventSystem : MonoBehaviour
{
    private static LevelEventSystem s_current = null;
    
    public static LevelEventSystem current
    {
        get
        {
            if (s_current == null) s_current = FindObjectOfType<LevelEventSystem>();
 
            return s_current;
        }

        set => s_current = value;
    }
    
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
