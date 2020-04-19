using System.Collections;
using System.Collections.Generic;
using DataTypes;
using UnityEngine;

public static class HelperMethods
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
    
    public static void DestroyObjects(IsWalkablePoint[] isWalkablePoints)
    {
        foreach (IsWalkablePoint isWalkablePoint in isWalkablePoints)
        {
            GameObject.DestroyImmediate(isWalkablePoint.gameObject);
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
    
    public static void CenterPosition(Transform targetTransform) => targetTransform.position = Vector3.zero;

    public static void SnapToGrid(Transform targetTransform)
    {
        Vector3 initPosition = targetTransform.position;
        initPosition.x = Mathf.RoundToInt(initPosition.x);
        initPosition.y = Mathf.RoundToInt(initPosition.y);
        initPosition.z = Mathf.RoundToInt(initPosition.z);

        targetTransform.position = initPosition;
    }
    
    public static Vector3 SnapToBlockGrid(Vector3 pos)
    {        
        Vector3 newPos = Vector3.zero;
            
        newPos.x = Mathf.RoundToInt(pos.x / Block.size.x) * Block.size.x;
        newPos.y = Mathf.RoundToInt(pos.y / Block.size.x) * Block.size.x;
        newPos.z = Mathf.RoundToInt(pos.z / Block.size.x) * Block.size.x;

        return newPos;
    }
    
    public static Vector3 SnapToBlockGridXZPlane(Vector3 pos)
    {
        Vector3 newPos = Vector3.zero;
            
        newPos.x = Mathf.RoundToInt(pos.x / Block.size.x) * Block.size.x;
        newPos.y = pos.y;
        newPos.z = Mathf.RoundToInt(pos.z / Block.size.x) * Block.size.x;

        return newPos;
    }
}