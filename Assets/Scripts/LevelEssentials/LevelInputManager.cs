using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInputManager : MonoBehaviour
{
    private DeviceType _deviceType;

    private Camera _mainCamera; //todo set to turnAroundCamera

    private void Awake()
    {
        _deviceType = SystemInfo.deviceType;
        _mainCamera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckBlockSelected();
    }

    void CheckBlockSelected()
    {
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
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            CheckIfGotBlock(ray);
        }
    }

    private void CheckTouchBlockTap()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.GetTouch(0).position);

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
                LevelEventSystem.current.OnBlockClicked(block.id);
            }
        }
    }
}
