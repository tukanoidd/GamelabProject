using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class LevelEnd : MonoBehaviour
{
    private Material _material;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
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
            SceneManager.LoadScene("LevelsMenu");
        }        
    }
}
