using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LoadLevelButton))]
public class LoadLevelButtonEditor : Editor
{
    private List<string> _levels;
    private int _scenesNum = 0;
    private int _selectedLevelIndex = 0;

    private LoadLevelButton _button;

    private void OnEnable()
    {
        _button = (LoadLevelButton) target;
        UpdateScenesList();
    }

    void UpdateScenesList()
    {
        _levels = new List<string>();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && !scene.path.Contains("Menu"))
            {
                string[] sceneName = scene.path.Split('/');
                
                _levels.Add(sceneName[sceneName.Length - 1].Split('.')[0]);
            }
        }
        
        _scenesNum = _levels.Count;
    }
    
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Update Scenes List"))
        {
            UpdateScenesList();
        }

        if (_scenesNum > 0 && _button)
        {
            _selectedLevelIndex = EditorGUILayout.Popup("Select A Level", _selectedLevelIndex, _levels.ToArray());
            _button.levelName = _levels[_selectedLevelIndex];
        }

        if (GUI.changed) EditorUtility.SetDirty(_button);
    }
}
