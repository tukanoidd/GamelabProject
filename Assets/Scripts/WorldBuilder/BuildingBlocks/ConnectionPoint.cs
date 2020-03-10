using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class ConnectionPoint : MonoBehaviour
{
    private SphereCollider _collider;
    private MeshRenderer _meshRenderer;
    private Material connectedMaterial;
    
    public static float scale = 0.05f;

    public bool drawDebugConnectionLines = true;

    public bool hasConnection = true;
    public bool isConnectedNearby = true;
    public bool hasCustomConnection = false;
    
    public float nearbyRadius = 0.5f;

    public ConnectionPoint connection;

    public Vector3 customCameraPoaition = Vector3.zero;
    public float customMaxOffset = 0.5f;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.radius = nearbyRadius;
        
        _meshRenderer = GetComponent<MeshRenderer>();
        connectedMaterial = Resources.Load<Material>("Materials/ConnectedConnectionPoint");

        InitConnectionsSettings();
    }

    private void InitConnectionsSettings()
    {
        if (!hasConnection)
        {
            if (isConnectedNearby) isConnectedNearby = false;
            if (hasCustomConnection) hasCustomConnection = false;
            if (connection) connection = null;
        }
        else
        {
            if (hasCustomConnection)
            {
                if (isConnectedNearby) isConnectedNearby = false;
            }
            else
            {
                if (!isConnectedNearby) isConnectedNearby = true;
                if (connection) connection = null;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (connection)
        {
            if (_meshRenderer && _meshRenderer.sharedMaterial != connectedMaterial) _meshRenderer.sharedMaterial = connectedMaterial;
            if (!connection.connection) connection.connection = this;
        }
        
    }

    private void OnCollisionStay(Collision other)
    {
        if (isConnectedNearby && !connection)
        {
            ConnectionPoint collidedConnectionPoint = other.gameObject.GetComponent<ConnectionPoint>();
            if (collidedConnectionPoint) connection = collidedConnectionPoint;
        }
    }

    private void OnDrawGizmos()
    {
        if (hasConnection && connection && drawDebugConnectionLines)
        {
#if UNITY_EDITOR
            Gizmos.DrawLine(transform.position, connection.transform.position);
#endif
        }
    }

    public void CustomPointConnect(ConnectionPoint customConnectionPoint)
    {
        connection = customConnectionPoint;
        customConnectionPoint.connection = this;
    }
}