using System.Collections;
using System.Collections.Generic;
using DataTypes;
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

    public static void DestroyObjects(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            GameObject.DestroyImmediate(collider.gameObject);
        }
    }
    
    public static Vector3 Multiply(Vector3 v1, Vector3 v2) => new Vector3(
        v1.x * v2.x,
        v1.y * v2.y,
        v1.z * v2.z
    );

    public static Vector3 Divide(Vector3 v1, Vector3 v2) => new Vector3(
        v1.x / v2.x,
        v1.y / v2.y,
        v1.z / v2.z
    );
    
    public static Vector3 Divide(Vector3 v1, BlockSize v2) => new Vector3(
        v1.x / v2.x,
        v1.y / v2.y,
        v1.z / v2.z
    );

    public static Vector3 Divide(BlockSize v1, Vector3 v2) => new Vector3(
        v1.x / v2.x,
        v1.y / v2.y,
        v1.z / v2.z
    );

    public static Vector3 Divide(BlockSize v1, BlockSize v2) => new Vector3(
        (float) v1.x / v2.x,
        (float) v1.y / v2.y,
        (float) v1.z / v2.z
    );
}