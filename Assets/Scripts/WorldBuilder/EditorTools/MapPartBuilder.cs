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
    
    [NonSerialized] public bool startingBlockCreated = false;

    #if UNITY_EDITOR
    public void CreateStartingBlock()
    {
        if (blockPrefab && !startingBlockCreated)
        {
            GameObject firstBlock = Instantiate(blockPrefab, transform);
            firstBlock.transform.localPosition = Vector3.zero;

            Selection.activeGameObject = firstBlock;

            startingBlockCreated = true;
        }
    } 
    #endif
}
