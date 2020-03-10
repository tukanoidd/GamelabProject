using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.ComponentModel;
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BuildingBlockEditor : Editor
{
    private Vector3 currHandlePos = Vector3.zero;
    private Matrix4x4 matrix = new Matrix4x4();

    private void OnSceneGUI()
    {
        Block targetBlock = (Block) target;

        if (targetBlock.transform.parent.name.ToLower().Contains("MapPartBuilder"))
        {
            int controlId = EditorGUIUtility.GetControlID("MoveBlock".GetHashCode(), FocusType.Keyboard);

            Event e = Event.current;

            switch (e.type)
            {
                case EventType.MouseDown:
                    //check nearest control and set hotControl/keyboardControl
                    if (HandleUtility.nearestControl == controlId && e.button == 0)
                    {
                        GUIUtility.hotControl = controlId;
                        GUIUtility.keyboardControl = controlId;
                        e.Use();

                        targetBlock.isCurrentBuildingBlock = true;
                    }

                    break;
                case EventType.MouseUp:
                    //check if i'm controlled and set hotControl/keyboardControl to 0
                    if (GUIUtility.hotControl == controlId && e.button == 0)
                    {
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }

                    break;
                case EventType.MouseDrag:
                    //if i'm controlled, move the point
                    break;
                case EventType.Repaint:
                    //draw point visual
                    break;
                case EventType.Layout:
                    //register distance from mouse to my point
                    
                    
                    break;
            }
        }
    }
}
#endif