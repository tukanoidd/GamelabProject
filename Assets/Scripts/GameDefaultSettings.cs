using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BlockSize
{
    public float xSize;
    public float ySize;
    public float zSize;

    public BlockSize(int aXSize, int aYSize, int aZSize)
    {
        xSize = aXSize;
        ySize = aYSize;
        zSize = aZSize;
    }
    
    public Vector3 ToVector()
    {
        return new Vector3(
            xSize,
            ySize,
            zSize
        );
    }
}

[CreateAssetMenu(fileName = "DefaultGameSettings", menuName = "ScriptableObjects/DefaultGameSettings")]
public class GameDefaultSettings : ScriptableObject
{
    [SerializeField] public BlockSize defaultBlockSize = new BlockSize(1, 1, 1);   
}
