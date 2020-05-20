using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DataTypes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Plane = DataTypes.Plane;

public static class HelperMethods
{
    public static void DestroyObjects(ConnectionPoint[] connectionPoints)
    {
        Debug.Log("Destroying connection point: " + DateTime.Now);
        foreach (ConnectionPoint connectionPoint in connectionPoints)
        {
            if (connectionPoint) GameObject.DestroyImmediate(connectionPoint.gameObject);
        }
    }

    public static void DestroyObjects(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            if (collider) GameObject.DestroyImmediate(collider);
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

    public static Vector3 SnapToBlockGridPlane(Vector3 pos, Plane plane)
    {
        Vector3 newPos = Vector3.zero;

        switch (plane)
        {
            case Plane.XY:
                newPos.x = Mathf.RoundToInt(pos.x / Block.size.x) * Block.size.x;
                newPos.y = Mathf.RoundToInt(pos.y / Block.size.y) * Block.size.y;
                newPos.z = pos.z;
                break;
            case Plane.XZ:
                newPos.x = Mathf.RoundToInt(pos.x / Block.size.x) * Block.size.x;
                newPos.y = pos.y;
                newPos.z = Mathf.RoundToInt(pos.z / Block.size.z) * Block.size.z;
                break;
            case Plane.YZ:
                newPos.x = pos.x;
                newPos.y = Mathf.RoundToInt(pos.y / Block.size.y) * Block.size.y;
                newPos.z = Mathf.RoundToInt(pos.z / Block.size.z) * Block.size.z;
                break;
        }

        return newPos;
    }

    public static bool CheckIsNear(ConnectionPoint connectionPoint1, ConnectionPoint connectionPoint2) =>
        Vector3.Distance(connectionPoint1.transform.position, connectionPoint2.transform.position) <=
        Block.nearbyRadius;

    public static bool CheckInGrid(Vector3 pos)
    {
        if (VectorIsInt(pos))
        {
            if (pos % Block.size == Vector3.zero)
            {
                return true;
            }
        }

        return false;
    }

    public static bool VectorIsInt(Vector3 vector) =>
        FloatIsInt(vector.x) && FloatIsInt(vector.y) && FloatIsInt(vector.z);

    public static bool FloatIsInt(float num) => Math.Abs(Mathf.RoundToInt(num) - num) < 0.05f;

    public static bool CheckBlockInPosition(Vector3 pos)
    {
        Block[] blocks = GameObject.FindObjectsOfType<Block>();

        foreach (Block block in blocks)
        {
            if (Vector3.Distance(block.transform.position, pos) <=
                Mathf.Min(Block.size.x, Block.size.y, Block.size.z) / 2f) return true;
        }

        return false;
    }

    public static bool KeyValuePairsEqualBothWays<T>(KeyValuePair<T, T> kVP1, KeyValuePair<T, T> kVP2)
    {
        if (kVP1.Key.Equals(kVP2.Key)) return kVP1.Value.Equals(kVP2.Value);
        if (kVP1.Key.Equals(kVP2.Value)) return kVP1.Value.Equals(kVP2.Key);
        if (kVP1.Value.Equals(kVP2.Key)) return kVP1.Key.Equals(kVP2.Value);
        if (kVP1.Value.Equals(kVP2.Value)) return kVP1.Key.Equals(kVP2.Key);

        return false;
    }

    public static void SaveLevelsProgress(LevelsProgressData dataToSave)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/levelsProgress.save");
        bf.Serialize(file, dataToSave);
        file.Close();
    }
}