using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public class MapPartBuilder : MonoBehaviour
{
    public GameObject blockPrefab;

#if UNITY_EDITOR
    public void CreateStartingBlock()
    {
        if (blockPrefab && !CheckIfStartingBlockExists())
        {
            GameObject firstBlock = Instantiate(blockPrefab, transform);
            firstBlock.name = "BuildingBlock " + (FindObjectsOfType<Block>().Length);
            firstBlock.transform.localPosition = Vector3.zero;

            Selection.activeGameObject = firstBlock;
        }
    }

    private bool CheckIfStartingBlockExists()
    {
        return GetComponentInChildren<Block>();
    }
    #endif
}
