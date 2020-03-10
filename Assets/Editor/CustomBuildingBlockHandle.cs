using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomBuildingBlockHandle : CustomHandle
{
    private int buildingBlockHash = "BuildingBlockHandle".GetHashCode();
    public Block block;
    public void DrawHandle(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 blockSize)
    {
        int controlId = EditorGUIUtility.GetControlID(buildingBlockHash, FocusType.Passive);
        Matrix4x4 trs = Matrix4x4.TRS(position, rotation, scale);

        Handles.FreeMoveHandle(controlId, position, rotation, 0f, blockSize, BuildingBlockHandleCap);
    }

    public void BuildingBlockHandleCap(int controlId, Vector3 position, Quaternion rotation, float size,
        EventType eventType)
    {
        if (eventType == EventType.Layout)
        {
            if (block)
            {
                Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                bool intersect = block.GetComponent<MeshFilter>().mesh.bounds.IntersectRay(mouseRay);
                if (intersect) HandleUtility.AddControl(controlId, 0);
                else HandleUtility.AddControl(controlId, 1e20f);
            }
        }
    }
}
