#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using DataTypes;
using Plane = DataTypes.Plane;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    private Player _targetPlayer;
    
    private void OnEnable()
    {
        _targetPlayer = (Player) target;
    }

    public override void OnInspectorGUI()
    {
        if (_targetPlayer)
        {
            if (GUILayout.Button("Snap To BLock Grid XZ Plane"))
            {
                _targetPlayer.transform.position =
                    HelperMethods.SnapToBlockGridPlane(_targetPlayer.transform.position, Plane.XZ);
            }
            
            if (GUILayout.Button("Snap To BLock Grid XY Plane"))
            {
                _targetPlayer.transform.position =
                    HelperMethods.SnapToBlockGridPlane(_targetPlayer.transform.position, Plane.XY);
            }
            
            if (GUILayout.Button("Snap To BLock Grid YZ Plane"))
            {
                _targetPlayer.transform.position =
                    HelperMethods.SnapToBlockGridPlane(_targetPlayer.transform.position, Plane.YZ);
            }

            if (GUILayout.Button("Update Rotation"))
            {
                _targetPlayer.UpdateRotation();
            }
        }

        DrawDefaultInspector();
    }
}
#endif