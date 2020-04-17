using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInputManager : MonoBehaviour
{
    //---------Public and Private Visible In Inspector---------\\
    //---------Public and Private Visible In Inspector---------\\

    //--------Private and Public Invisible In Inspector--------\\
    private TurnAroundCamera _mainCamera;
    //--------Private and Public Invisible In Inspector--------\\

    private void Awake()
    {
        _mainCamera = FindObjectOfType<TurnAroundCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckBlockSelected();
    }

    void CheckBlockSelected()
    {
        switch (GameManager.current.deviceType)
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
                LevelEventSystem.current.OnBlockClicked(block.id);
            }
        }
    }
}