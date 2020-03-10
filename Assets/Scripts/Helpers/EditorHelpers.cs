using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class EditorHelpers
    {
        public static void CenterPosition(Transform targetTransform) => targetTransform.position = Vector3.zero;

        public static void SnapToGrid(Transform targetTransform)
        {
            Vector3 initPosition = targetTransform.position;
            initPosition.x = Mathf.RoundToInt(initPosition.x);
            initPosition.y = Mathf.RoundToInt(initPosition.y);
            initPosition.z = Mathf.RoundToInt(initPosition.z);

            targetTransform.position = initPosition;
        }

        public static void SnapToBlockGrid(Transform targetTransform, float sizeX, float sizeY, float sizeZ)
        {        
            Vector3 newPosition = targetTransform.position;
            newPosition.x = Mathf.RoundToInt(newPosition.x / sizeX) * sizeX;
            newPosition.y = Mathf.RoundToInt(newPosition.y / sizeY) * sizeY;
            newPosition.z = Mathf.RoundToInt(newPosition.z / sizeY) * sizeY;

            targetTransform.position = newPosition;
        }
    }   
}