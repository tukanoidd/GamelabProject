using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevelButton : MonoBehaviour
{
    [SerializeField] private Image frame;
    
    [HideInInspector] public int selectedLevelIndex = 0;
    [HideInInspector] public string levelName = "";
    [HideInInspector] public bool lastLevel = false;
    
    private Settings _settings;

    private void Start()
    {
        _settings = Resources.Load<Settings>("ScriptableObjects/Settings");
        frame.gameObject.SetActive(lastLevel);
    }

    public void LoadLevel()
    {
        if (levelName.Contains("1") && !_settings.cutScenesPlayed) SceneManager.LoadScene("CutScenes");   
        else SceneManager.LoadScene(levelName);
    }
}
