using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapPartBuilder : MonoBehaviour
{
    public GameObject blockPrefab;
    
    [NonSerialized] public bool startingBlockCreated = false;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateStartingBlock()
    {
        if (blockPrefab && !startingBlockCreated)
        {
            GameObject firstBlock = Instantiate(blockPrefab, transform);
            firstBlock.transform.localPosition = Vector3.zero;

            startingBlockCreated = true;
        }
    } 
}
