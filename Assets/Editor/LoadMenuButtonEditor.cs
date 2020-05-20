#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LoadMenuButton))]
public class LoadMenuButtonEditor : Editor
{
    private List<string> _menus;
    private int _scenesNum = 0;

    private LoadMenuButton _button;

    private void OnEnable()
    {
        _button = (LoadMenuButton) target;
        UpdateScenesList();
    }

    void UpdateScenesList()
    {
        _menus = new List<string>();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && scene.path.Contains("Menu"))
            {
                string[] sceneName = scene.path.Split('/');
                
                _menus.Add(sceneName[sceneName.Length - 1].Split('.')[0]);
            }
        }
        
        _scenesNum = _menus.Count;
    }
    
    public override void OnInspectorGUI()
    {
        if (_button)
        {
            if (GUILayout.Button("Update Scenes List"))
            {
                UpdateScenesList();
            }

            if (_scenesNum > 0 && _button)
            {
                _button.selectedMenuIndex = EditorGUILayout.Popup("Select A Level", _button.selectedMenuIndex, _menus.ToArray());
                _button.menuName = _menus[_button.selectedMenuIndex];
            }
            
            if (GUI.changed) EditorUtility.SetDirty(_button);
        }

        DrawDefaultInspector();
    }
}
#endif