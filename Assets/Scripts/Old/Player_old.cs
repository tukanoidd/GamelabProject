using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using Helpers;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PathFinder))]
public class Player_old : MonoBehaviour
{

    /*private void Update()
    {
        if (!gamePaused)
        {
            if (!isMoving) Debug.Log("not moving");
            else
            {
                if (_targetPosition.HasValue && !teleporting)
                {
                    transform.LookAt(_targetPosition.Value);
                    transform.position = Vector3.MoveTowards(transform.position, _targetPosition.Value,
                        walkSpeed * Time.deltaTime);

                    //if (Vector3.Distance(transform.position, _targetPosition.Value) < 0.01f) MovePath(_current.parent);
                }
            }
        }
    }*/

    /*public void MovePath(Location currentBlock)
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
    }*/

    /*void StopMovement()
    {
        isMoving = false;
        _targetPosition = null;
        //_current = null;
    }*/
}