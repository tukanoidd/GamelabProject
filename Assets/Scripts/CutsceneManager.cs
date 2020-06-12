using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> cutScenes = new List<Sprite>();
    [SerializeField] private List<string> cutSceneTexts = new List<string>();
    
    [SerializeField] private string gotoLevel = "Level 1";
    
    private Image _cutSceneImage;
    private ParticleSystem _particles;
    private TextMeshProUGUI _cutSceneText;

    private Settings _settings;

    private int _selectedCutScene = 0;
    
    private void Awake()
    {
        _cutSceneImage = GetComponent<Image>();
        _particles = GetComponent<ParticleSystem>();

        _cutSceneText = transform.Find("CutSceneText").GetComponent<TextMeshProUGUI>();

        _settings = Resources.Load<Settings>("ScriptableObjects/Settings");
        
        ChangeCutScene(true);
    }

    private void Update()
    {
        if (GameManager.current.deviceType == DeviceType.Handheld && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) NextScene();
        }
        else if (GameManager.current.deviceType == DeviceType.Desktop && Input.GetKeyDown(KeyCode.Space)) NextScene();
    }

    private void ChangeCutScene(bool playParticles = false)
    {
        if (cutScenes.Any() && cutSceneTexts.Any() && cutScenes.Count == cutSceneTexts.Count)
        {
            _cutSceneImage.sprite = cutScenes[_selectedCutScene];
            _cutSceneText.text = cutSceneTexts[_selectedCutScene];
        
            if (playParticles) _particles.Play();
            else _particles.Stop();   
        }
        else Debug.LogError("Either one of the lists is Empty or they're not the same Length");
    }

    private void NextScene()
    {
        _selectedCutScene++;

        if (_selectedCutScene >= cutScenes.Count)
        {
            _settings.cutScenesPlayed = true;
            PlayerPrefs.SetInt("cutScenesPlayed", 1);
            PlayerPrefs.Save();
            
            SceneManager.LoadScene(gotoLevel);
        }
        else ChangeCutScene();
    }
}