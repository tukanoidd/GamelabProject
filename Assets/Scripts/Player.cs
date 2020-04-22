﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AStarPathFinding;
using Helpers;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PathFinder))]
public class Player : MonoBehaviour
{
    [SerializeField] private Vector3 gravity = Vector3.down;
    [SerializeField] private float walkSpeed = 10;
    [SerializeField] private bool drawPathWhenMoving = true;

    private DeviceType _deviceType;
    private GameDefaultSettings _defaultGameSettings;

    private PathFinder _pathFinder;
    private CharacterController _characterController;
    private TurnAroundCamera _mainCamera;

    private Vector3 _gravitySpeed = Vector3.zero;
    private Vector3? _targetPosition = null;
    private Block _targetBlock = null;
    private Location _current = null;

    private float _height = 0;
    private bool _heightChecked = false;

    private Material _testBlockMat;

    [NonSerialized] public bool isMoving = false;
    [NonSerialized] public bool teleporting = false;
    [NonSerialized] public bool gamePaused = false;

    private void Awake()
    {
        _deviceType = SystemInfo.deviceType;

        _pathFinder = GetComponent<PathFinder>();
        _characterController = GetComponent<CharacterController>();

        _mainCamera = FindObjectOfType<TurnAroundCamera>();

        _testBlockMat = Resources.Load<Material>("Materials/PathFindingBlockTest");
        _defaultGameSettings = Resources.Load<GameDefaultSettings>("ScriptableObjects/DefaultGameSettings");
    }

    private void Update()
    {
        if (!gamePaused)
        {
            if (_characterController.isGrounded)
            {
                if (!_heightChecked) CheckHeight();
            }
            else ApplyGravity();

            if (!isMoving) CheckBlockSelected();
            else
            {
                if (_targetPosition.HasValue && !teleporting)
                {
                    transform.LookAt(_targetPosition.Value);
                    transform.position = Vector3.MoveTowards(transform.position, _targetPosition.Value,
                        walkSpeed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, _targetPosition.Value) < 0.01f) MovePath(_current.parent);
                }
            }

            LockRotation();
        }
    }

    private void LockRotation()
    {
        Vector3 newRot = transform.rotation.eulerAngles;
        newRot.x = 0;
        newRot.z = 0;

        transform.rotation = Quaternion.Euler(newRot);
    }

    private void CheckHeight()
    {
        RaycastHit hit;

        if (Physics.Raycast(new Ray(transform.position, -transform.up), out hit, 1000, LayerMask.NameToLayer("Map")))
        {
            if (hit.transform.gameObject.GetComponent<Block>())
            {
                _height = Vector3.Distance(transform.position, hit.point);
                _heightChecked = true;
            }
        }
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded) _gravitySpeed = Vector3.zero;

        _gravitySpeed += gravity * Time.deltaTime;

        _characterController.Move(_gravitySpeed * Time.deltaTime);
    }

    private void CheckBlockSelected()
    {
        if (!_mainCamera) return;
        if (!_mainCamera.cam) return;

        switch (_deviceType)
        {
            case DeviceType.Desktop:
                CheckMouseBlockClick();
                break;
            case DeviceType.Handheld:
                CheckTouchBlockTap();
                break;
        }
    }

    private void CheckMouseBlockClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.cam.ScreenPointToRay(Input.mousePosition);

            CheckIfGotBlock(ray);
        }
    }

    private void CheckTouchBlockTap()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = _mainCamera.cam.ScreenPointToRay(Input.GetTouch(0).position);

            CheckIfGotBlock(ray);
        }
    }

    private void CheckIfGotBlock(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, LayerMask.NameToLayer("Map")))
        {
            Block block = hit.transform.GetComponent<Block>();
            if (block)
            {
                MapBlockData blockData = block.thisBlocksMapData;
                if (blockData != null) _pathFinder.GetMovementInstructions(blockData, this);
            }
        }
    }

    public void MovePath(Location currentBlock)
    {
        if (currentBlock?.coords.blockCoords != null && _heightChecked && _pathFinder.currBlockCoords.HasValue)
        {
            isMoving = true;
            Vector3 blockCoords = currentBlock.coords.blockCoords.Value;
            _targetPosition = new Vector3(blockCoords.x,
                blockCoords.y + _height + _defaultGameSettings.defaultBlockSize.ySize / 2, blockCoords.z);

            Block block = _pathFinder.mapData.map[currentBlock.coords.mapCoords.x, currentBlock.coords.mapCoords.z]
                .block;

            if (block)
            {
                _targetBlock = block;
                Location next = currentBlock.parent;

                if (next != null)
                {
                    Block nextBlock = _pathFinder.mapData.map[next.coords.mapCoords.x, next.coords.mapCoords.z].block;

                    ConnectionPoint conPoint = block.connectionPoints.FirstOrDefault(cP =>
                        cP.hasCustomConnection && cP.connection &&
                        cP.connection.parentBlock == nextBlock
                    );

                    if (conPoint != null)
                    {
                        if (conPoint.customCameraPositions.Any(camPos =>
                            Vector3.Distance(camPos, _mainCamera.transform.position) > conPoint.customMaxCamOffset))
                        {
                            StopMovement();
                            return;
                        }
                        else goto ContinueMoving;
                    }

                    conPoint = block.connectionPoints.FirstOrDefault(cP =>
                        cP.isConnectedNearby && cP.connection &&
                        cP.connection.parentBlock == nextBlock
                    );

                    if (conPoint == null)
                    {
                        StopMovement();
                        return;
                    }
                }

                ContinueMoving:

                if (_testBlockMat && drawPathWhenMoving)
                {
                    MeshRenderer blockMeshRenderer = block.GetComponent<MeshRenderer>();
                    Material ogMat = blockMeshRenderer.material;
                    blockMeshRenderer.material = _testBlockMat;
                }

                _current = currentBlock;
                return;
            }
        }

        StopMovement();
    }

    void StopMovement()
    {
        isMoving = false;
        _targetPosition = null;
        _current = null;
    }

    public void TeleportToConPoint(ConnectionPoint conPoint, Vector3 offset)
    {
        if (_targetPosition.HasValue && _targetBlock)
        {
            if (_targetBlock == conPoint.parentBlock)
            {
                Debug.Log(_targetBlock.name);
                Debug.Log(conPoint.parentBlock.name);
                teleporting = true;

                transform.position = conPoint.transform.position + offset;
                teleporting = false;   
            }
        }
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (_targetPosition.HasValue)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _targetPosition.Value);
        }

        Gizmos.color = Color.green;
        Vector3 lineStart = transform.position + Vector3.up * _height / 2;
        Gizmos.DrawLine(lineStart, lineStart + transform.forward * 2);
#endif
    }
}