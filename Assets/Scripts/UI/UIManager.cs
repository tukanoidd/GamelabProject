using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private float levelButtonWidth = 400;
    [SerializeField] private float levelButtonHeight = 130;
    
    [SerializeField] private int levelsCount = 3;
    [SerializeField] private List<LoadLevelButton> levelButtons;
    
    private LevelsProgress _levelsProgress;
    private GameObject _levelButtonsHolder;

    private Settings _settings;
    
    private void Awake()
    {
        _levelButtonsHolder = GameObject.Find("LevelButtons");
        _levelsProgress = Resources.Load<LevelsProgress>("ScriptableObjects/LevelsProgress");

        _settings = Resources.Load<Settings>("ScriptableObjects/Settings");

        if (!PlayerPrefs.HasKey("cutScenesPlayed"))
        {
            PlayerPrefs.SetInt("cutScenesPlayed", 0);
            _settings.cutScenesPlayed = false;
            PlayerPrefs.Save();
        }
        else _settings.cutScenesPlayed = PlayerPrefs.GetInt("cutScenesPlayed") != 0;

        LoadLevelsProgress();

        if (SceneManager.GetActiveScene().name == "LevelsMenu")
        {
            levelButtons = new List<LoadLevelButton>();
            foreach (Transform child in _levelButtonsHolder.transform)
            {
                LoadLevelButton button = child.gameObject.GetComponent<LoadLevelButton>();
                button.lastLevel = false;
                levelButtons.Add(button);
                child.gameObject.SetActive(_levelsProgress.levelsUnlocked.Any(lName => button.levelName.Contains(lName.Trim())));
            }
            
            levelButtons.First(button => button.levelName.Contains(_levelsProgress.levelsUnlocked.Last())).lastLevel =
                true;
        }
    }

    private void LoadLevelsProgress()
    {
        if (!_levelsProgress.allLevels.Any() || _levelsProgress.levelsUnlocked.Count() < 2)
        {
            if (File.Exists(Application.persistentDataPath + "/levelsProgress.save"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/levelsProgress.save", FileMode.Open);
                LevelsProgressData levelsProgressData = (LevelsProgressData) bf.Deserialize(file);
                _levelsProgress.allLevels = levelsProgressData.allLevels;
                _levelsProgress.levelsUnlocked = levelsProgressData.levelsUnlocked;
                file.Close();
            }
            else
            {
                LevelsProgressData dataToSave = CreateNewLevelsProgressDataObject();
                _levelsProgress.allLevels = dataToSave.allLevels;
                _levelsProgress.levelsUnlocked = dataToSave.levelsUnlocked;
                HelperMethods.SaveLevelsProgress(dataToSave);
            }
        }
    }

    private LevelsProgressData CreateNewLevelsProgressDataObject()
    {
        LevelsProgressData lProgData = new LevelsProgressData();
        
        lProgData.allLevels = new List<string>();

        for (int i = 0; i < levelsCount; i++)
        {
            lProgData.allLevels.Add("Level " + (i + 1));
        }
        
        lProgData.levelsUnlocked = new List<string>() {lProgData.allLevels[0]};

        return lProgData;
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
}