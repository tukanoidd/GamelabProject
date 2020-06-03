using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SettingsUIManager : MonoBehaviour
{
    private Settings _settings;

    [Space(20)] 
    [SerializeField] private bool isMainMenu;

    [Header("References")] 
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Image soundButtonImage;
    [SerializeField] private GameObject backButton;
    
    private Sprite _soundButtonSpriteOn;
    private Sprite _soundButtonSpriteOff;

    void Awake()
    {
        _settings = Resources.Load<Settings>("ScriptableObjects/Settings");
        _soundButtonSpriteOff = Resources.Load<Sprite>("UI/SoundButtonOff");
        _soundButtonSpriteOn = Resources.Load<Sprite>("UI/SoundButtonOn");
        
        if (!PlayerPrefs.HasKey("soundOn"))
        {
            PlayerPrefs.SetInt("soundOn", 1);
            _settings.soundOn = true;
            PlayerPrefs.Save();
        }
        else _settings.soundOn = PlayerPrefs.GetInt("soundOn") != 0;

        UpdateSoundButton();
        settingsPanel.SetActive(false);
        if (isMainMenu) backButton.SetActive(false);
    }

    private void UpdateSoundButton()
    {
        soundButtonImage.sprite = _settings.soundOn ? _soundButtonSpriteOn : _soundButtonSpriteOff;
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