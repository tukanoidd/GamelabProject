using System;
using UnityEditor;
using UnityEngine;
using GravitationalPlane = DataTypes.GravitationalPlane;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class IsWalkablePoint : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    public static float scale = 0.25f;

    public GravitationalPlane gravitationalPlane;

    public bool isWalkable = true;
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
#if UNITY_EDITOR
    private MeshRenderer _meshRenderer;
    private Material _isWalkableMat;
    private Material _isNotWalkableMat;
#endif

    [NonSerialized] public Block parentBlock;
    //--------Private and Public Invisible In Inspector--------\\

    private void Awake()
    {
#if UNITY_EDITOR
        _meshRenderer = GetComponent<MeshRenderer>();

        _isWalkableMat = Resources.Load<Material>("Materials/WalkableBlockPointMat");
        _isNotWalkableMat = Resources.Load<Material>("Materials/NotWalkableBlockPointMat");

        _meshRenderer.sharedMaterial = isWalkable ? _isWalkableMat : _isNotWalkableMat;
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (isWalkable && _meshRenderer.sharedMaterial != _isWalkableMat) _meshRenderer.sharedMaterial = _isWalkableMat;
        else if (!isWalkable && _meshRenderer.sharedMaterial != _isNotWalkableMat)
            _meshRenderer.sharedMaterial = _isNotWalkableMat;
#endif
    }
}