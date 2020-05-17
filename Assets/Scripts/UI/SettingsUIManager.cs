using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SettingsUIManager : MonoBehaviour
{
    [SerializeField] private Settings settings;
    
    [Space(20)]
    [SerializeField] private bool isMainMenu = false;

    [Header("References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject soundButton;
    [SerializeField] private Text soundButtonText;
    [SerializeField] private GameObject backButton;

    void Awake()
    {
        UpdateSoundButton();
        settingsPanel.SetActive(false);
        if (isMainMenu) backButton.SetActive(false);
    }

    private void UpdateSoundButton()
    {
        soundButtonText.text = "M - " + (settings.soundOn ? "On" : "Off");
        Debug.Log(soundButtonText.text);
    }

    public void ToggleSound()
    {
        settings.soundOn = !settings.soundOn;
        UpdateSoundButton();
    }

    public void TogleSettings()
    {
        bool newVisibility = !settingsPanel.activeSelf;
        
        if (settingsPanel) settingsPanel.SetActive(newVisibility);
    }
}