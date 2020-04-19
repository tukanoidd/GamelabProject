using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
public class UIManager : MonoBehaviour
{
    [SerializeField] private bool isLevel = false;
    [SerializeField] bool isVisible = true;

    private Canvas _canvas;
    private GameObject _mainPanel;
    private GameObject _pauseButton;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _mainPanel = GameObject.FindGameObjectWithTag("MainPanel");
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
        if (GameManager.current.deviceType == DeviceType.Desktop)
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

        GameManager.current.gamePaused = isVisible;

        if (_pauseButton) _pauseButton.SetActive(!isVisible);
    }
}