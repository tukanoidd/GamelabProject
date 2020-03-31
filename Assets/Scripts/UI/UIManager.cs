using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class UIManager : MonoBehaviour
{
    [SerializeField] private bool isLevel = false;
    [SerializeField] bool isVisible = true;

    private DeviceType _deviceType;
    
    private Canvas _canvas;
    private Player _player;
    private TurnAroundCamera _cam;
    private GameObject _mainPanel;
    private GameObject _pauseButton;

    void Awake()
    {
        _deviceType = SystemInfo.deviceType;

            _canvas = GetComponent<Canvas>();
        
        _mainPanel = GameObject.FindGameObjectWithTag("MainPanel");

        _player = FindObjectOfType<Player>();
        _cam = FindObjectOfType<TurnAroundCamera>();
        
        _pauseButton = GameObject.FindGameObjectWithTag("PauseButton");

        SetPause();
    }
    public void LevelsMenu()
    {
        SceneManager.LoadScene("LevelsMenu");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Update()
    {
        if (_deviceType == DeviceType.Desktop)
        {
            if (isLevel && _canvas)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    ToggleVisibility();
                }   
            }   
        }
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;
        
        SetPause();
    }

    void SetPause()
    {
        if (_mainPanel) _mainPanel.SetActive(isVisible);
        
        if (_player) _player.gamePaused = isVisible;
        if (_cam) _cam.gamePaused = isVisible;
        
        if (_pauseButton) _pauseButton.SetActive(!isVisible);
    }
}
