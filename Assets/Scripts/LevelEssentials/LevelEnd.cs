using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class LevelEnd : MonoBehaviour
{
    private Material _material;
    private LevelsProgress _levelsProgress;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
        _levelsProgress = Resources.Load<LevelsProgress>("ScriptableObjects/LevelsProgress");
    }

    void Start()
    {
        if (_material) StartCoroutine(AnimateMaterial());
    }

    private IEnumerator AnimateMaterial()
    {
        float duration = 3f; 
        float normalizedTime = 0;
        while(normalizedTime <= 1f)
        {
            _material.SetFloat("Vector1_8C9F465C", normalizedTime);
            _material.SetFloat("Vector1_DE47FC80", normalizedTime);
            
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        StartCoroutine(AnimateMaterial());
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            UnlockNewLevel();
            SceneManager.LoadScene("LevelsMenu");
        }        
    }

    private void UnlockNewLevel()
    {
        int nextLevel = int.Parse(SceneManager.GetActiveScene().name.Split(' ')[1]);

        if (nextLevel >= _levelsProgress.allLevels.Count) return;
        
        if (_levelsProgress.levelsUnlocked.Any(lName => lName.Contains((nextLevel + 1).ToString()))) return;
        _levelsProgress.levelsUnlocked.Add(_levelsProgress.allLevels[nextLevel]);
        _levelsProgress.levelsUnlocked.Sort();

        HelperMethods.SaveLevelsProgress(_levelsProgress.ToLevelsProgressData());
    }
}
