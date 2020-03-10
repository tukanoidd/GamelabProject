﻿using UnityEngine;

public class CustomHandle
{
    Quaternion rotation = Quaternion.identity;
    Vector3 scale = Vector3.one;
    private Vector3 position;

    public Matrix4x4 matrix = Matrix4x4.identity;

    public Event e
    {
        get { return Event.current; }
    }

    public void SetTransform(Transform transform)
    {
        rotation = transform.rotation;
        position = transform.position;
        scale = transform.localScale;
        matrix = Matrix4x4.TRS(position, rotation, scale);
    }

    public void SetTransform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.rotation = rotation;
        this.position = position;
        this.scale = scale;
        matrix = Matrix4x4.TRS(position, rotation, scale);
    }

    public void SetTransform(CustomHandle handle)
    {
        rotation = Quaternion.LookRotation(handle.matrix.GetColumn(2), handle.matrix.GetColumn(1));
        position = matrix.GetColumn(3);
        scale = new Vector3(
            handle.matrix.GetColumn(0).magnitude,
            handle.matrix.GetColumn(1).magnitude,
            handle.matrix.GetColumn(2).magnitude
        );
        
        matrix = Matrix4x4.TRS(position, rotation, scale);
    }
}