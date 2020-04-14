using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectsHelpers
{
    public static void DestroyObjects(ConnectionPoint[] connectionPoints)
    {
        foreach (ConnectionPoint connectionPoint in connectionPoints)
        {
            GameObject.DestroyImmediate(connectionPoint.gameObject);
        }
    }
} 