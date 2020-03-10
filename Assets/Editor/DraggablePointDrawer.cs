using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(MonoBehaviour), true)]
public class DraggablePointDrawer : Editor
{
    readonly GUIStyle style = new GUIStyle();

    private void OnEnable()
    {
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
    }

    public void OnSceneGUI()
    {
        var property = serializedObject.GetIterator();

        while (property.Next(true))
        {
            if (property.propertyType == SerializedPropertyType.Vector3)
            {
                FieldInfo field = serializedObject.targetObject.GetType().GetField(property.name);
                
                if (field == null) continue;

                object[] draggablePoints = field.GetCustomAttributes(typeof(DraggablePoint), false);

                if (draggablePoints.Length > 0)
                {
                    Handles.Label(property.vector3Value, property.name);
                    property.vector3Value = Handles.PositionHandle(property.vector3Value, Quaternion.identity);
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
#endif