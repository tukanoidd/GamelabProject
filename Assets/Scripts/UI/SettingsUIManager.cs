using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SettingsUIManager : MonoBehaviour
{
    private Settings _settings;

    [Space(20)] [SerializeField] private bool isMainMenu = false;

    [Header("References")] [SerializeField]
    private GameObject settingsPanel;

    [SerializeField] private GameObject soundButton;
    [SerializeField] private Text soundButtonText;
    [SerializeField] private GameObject backButton;

    void Awake()
    {
        _settings = Resources.Load<Settings>("ScriptableObjects/Settings");
        
        if (!PlayerPrefs.HasKey("soundOn"))
        {
            PlayerPrefs.SetInt("soundOn", 1);
            _settings.soundOn = true;
            PlayerPrefs.Save();
        }
        else
        {
            if (PlayerPrefs.GetInt("soundOn") == 0) _settings.soundOn = false;
            else _settings.soundOn = true;
        }

        UpdateSoundButton();
        settingsPanel.SetActive(false);
        if (isMainMenu) backButton.SetActive(false);
    }

    private void UpdateSoundButton()
    {
        soundButtonText.text = "M - " + (_settings.soundOn ? "On" : "Off");
    }

    public void ToggleSound()
    {
        _settings.soundOn = !_settings.soundOn;
        PlayerPrefs.SetInt("soundOn", _settings.soundOn ? 1 : 0);
        PlayerPrefs.Save();
        
        UpdateSoundButton();
    }

    public void TogleSettings()
    {
        bool newVisibility = !settingsPanel.activeSelf;

        if (settingsPanel) settingsPanel.SetActive(newVisibility);
    }
}