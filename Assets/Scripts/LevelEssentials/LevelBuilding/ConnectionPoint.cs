using System;
using System.Collections.Generic;
using DataTypes;
using UnityEngine;
using Plane = UnityEngine.Plane;

public class ConnectionPoint : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    public static float scale = 0.1f;

    public bool drawDebugConnectionLines = true;

    public float nearbyRadius = 1;

    public List<Vector3> customCameraPositions = new List<Vector3>();
    public float customMaxCamOffset = 0.5f;
    
    public bool HasConnections => connections.Count > 0;
    
    public BoxCollider TpTrigger
    {
        set => _tpTrigger = value;
    } 
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    private MeshRenderer _meshRenderer;
    private Material _standardMaterial;
    private Material _connectedMaterial;
    private Player _player;

    private BoxCollider _tpTrigger;

    [NonSerialized] public Block parentBlock;
    [NonSerialized] public List<ConnectionPoint> connections;
    
    [NonSerialized] public Dictionary<Plane, AxisPositionDirection> posDirs;
    //--------Private and Public Invisible In Inspector--------\\

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}