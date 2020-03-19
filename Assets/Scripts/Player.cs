using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PathFinder))]
public class Player : MonoBehaviour
{
    [SerializeField] private Vector3 gravity = Vector3.down;

    private DeviceType _deviceType;

    private PathFinder _pathFinder;
    private CharacterController _characterController;
    private TurnAroundCamera _mainCamera;

    private Vector3 _speed = Vector3.zero;

    private void Awake()
    {
        _deviceType = SystemInfo.deviceType;

        _pathFinder = GetComponent<PathFinder>();
        _characterController = GetComponent<CharacterController>();

        _mainCamera = FindObjectOfType<TurnAroundCamera>();
    }

    private void Update()
    {
        ApplyGravity();

        CheckBlockSelected();
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded) _speed = Vector3.zero;

        _speed += gravity * Time.deltaTime;

        _characterController.Move(_speed * Time.deltaTime);
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
                Debug.Log("clicked block");
                MapBlockData blockData = block.thisBlocksMapData;
                if (blockData != null) _pathFinder.MoveToward(blockData);
            }
        }
    }
}