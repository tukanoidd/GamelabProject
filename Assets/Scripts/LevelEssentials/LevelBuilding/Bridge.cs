using System;
using System.Collections;
using System.Collections.Generic;
using DataTypes;
using UnityEngine;

[ExecuteAlways]
public class Bridge : MonoBehaviour
{
    [HideInInspector]
    public AxisPositionDirection bridgeOrientation = new AxisPositionDirection(Axis.X, AxisDirection.Positive);

    [Space(20)] [SerializeField] private GameObject bridgeEndPrefab;
    [SerializeField] private GameObject bridgeMiddlePrefab;

    [Range(0, 10)] [SerializeField] private int middleLength = 0;

    [HideInInspector] public Transform bridgeFirstEndObjectWithBlock;
    private Transform _bridgeSecondEndObjectWithBlock;

    private Transform _bridgeFirstEnd;
    private Transform _bridgeSecondEnd;
    private Transform _bridgeMiddlePartsHolder;
    private List<Transform> _bridgeMiddleParts;

    private void Awake()
    {
        if (bridgeEndPrefab == null) bridgeEndPrefab = Resources.Load<GameObject>("Prefabs/Bridges/BridgeEnd");
        if (bridgeMiddlePrefab == null) bridgeEndPrefab = Resources.Load<GameObject>("Prefabs/Bridges/BridgeMiddle");

        if (Application.isEditor) CreateBridge();
    }

    public void CreateBridge()
    {
        //Removing all the children
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        children.ForEach(child => DestroyImmediate(child));
        GameManager.current.UpdateConnections();

        int dirInt = AxisPositionDirection.NormalizeDirection(bridgeOrientation.dir);

        //First End
        bridgeFirstEndObjectWithBlock = Instantiate(bridgeEndPrefab, transform).transform;
        bridgeFirstEndObjectWithBlock.localPosition = Vector3.zero;
        bridgeFirstEndObjectWithBlock.name = "FirstEnd";

        _bridgeFirstEnd = bridgeFirstEndObjectWithBlock.Find("BridgeEnd");

        Vector3 firstEndRotation = GetBridgeEndRotation();
        _bridgeFirstEnd.localEulerAngles = firstEndRotation;

        // Middle Parts
        Vector3 bridgeMiddlePartsHolderPosition = Vector3.zero;
        if (bridgeOrientation.axis == Axis.X) bridgeMiddlePartsHolderPosition.x += Block.size.x * dirInt;
        else if (bridgeOrientation.axis == Axis.Z) bridgeMiddlePartsHolderPosition.z += Block.size.z * dirInt;

        _bridgeMiddlePartsHolder = new GameObject().transform;
        _bridgeMiddlePartsHolder.parent = transform;
        _bridgeMiddlePartsHolder.localPosition = bridgeMiddlePartsHolderPosition;
        _bridgeMiddlePartsHolder.name = "MiddleParts";

        _bridgeMiddleParts = new List<Transform>();
        Vector3 bridgeMiddlePartRotation = GetBridgeMiddlePartRotation();
        Vector3 offset = Vector3.zero;

        if (bridgeOrientation.axis == Axis.X) offset.x += Block.size.x * dirInt;
        else if (bridgeOrientation.axis == Axis.Z) offset.z += Block.size.z * dirInt;

        for (int i = 0; i < middleLength; i++)
        {
            GameObject bridgeMiddlePartWithBlock = Instantiate(bridgeMiddlePrefab, _bridgeMiddlePartsHolder);
            bridgeMiddlePartWithBlock.transform.localPosition = i * offset;
            bridgeMiddlePartWithBlock.name = "BridgeMiddle " + (i + 1);

            Transform bridgeMiddlePart = bridgeMiddlePartWithBlock.transform.Find("BridgeMid");
            bridgeMiddlePart.localEulerAngles = bridgeMiddlePartRotation;

            _bridgeMiddleParts.Add(bridgeMiddlePart);
        }

        //Second Part
        offset = Block.size.ToVector() * (_bridgeMiddleParts.Count + 1);

        Vector3 secondEndPosition = new Vector3(0, 0, 0);
        if (bridgeOrientation.axis == Axis.X) secondEndPosition.x += offset.x * dirInt;
        else if (bridgeOrientation.axis == Axis.Z) secondEndPosition.z += offset.z * dirInt;

        _bridgeSecondEndObjectWithBlock =
            Instantiate(bridgeEndPrefab, transform).transform;
        _bridgeSecondEndObjectWithBlock.localPosition = secondEndPosition;
        _bridgeSecondEndObjectWithBlock.name = "SecondEnd";

        _bridgeSecondEnd = _bridgeSecondEndObjectWithBlock.Find("BridgeEnd");

        Vector3 secondEndRotation = GetBridgeEndRotation(false);
        _bridgeSecondEnd.localEulerAngles = secondEndRotation;
    }

    private Vector3 GetBridgeEndRotation(bool firstEnd = true)
    {
        Vector3 newLocalRotation = new Vector3(180, 0, 0);

        if (bridgeOrientation.axis == Axis.X)
        {
            if (bridgeOrientation.dir == AxisDirection.Positive) newLocalRotation.y = firstEnd ? 0 : 180;
            else if (bridgeOrientation.dir == AxisDirection.Negative) newLocalRotation.y = firstEnd ? 180 : 0;
        } else if (bridgeOrientation.axis == Axis.Z)
        {
            if (bridgeOrientation.dir == AxisDirection.Positive) newLocalRotation.y = firstEnd ? -90 : 90;
            else if (bridgeOrientation.dir == AxisDirection.Negative) newLocalRotation.y = firstEnd ? 90 : -90;
        }

        return newLocalRotation;
    }

    private Vector3 GetBridgeMiddlePartRotation() => new Vector3(
        -90,
        bridgeOrientation.axis == Axis.X ? 90 : 0,
        -90
    );
}