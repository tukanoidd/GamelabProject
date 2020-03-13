using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

        public static Vector3 SnapToBlockGrid(Vector3 pos, Vector3 size)
        {        
            Vector3 newPos = Vector3.zero;
            
            newPos.x = Mathf.RoundToInt(pos.x / size.x) * size.x;
            newPos.y = Mathf.RoundToInt(pos.y / size.x) * size.x;
            newPos.z = Mathf.RoundToInt(pos.z / size.x) * size.x;

            return newPos;
        }
        
        #if UNITY_EDITOR
        public static bool ToolsHidden {
            get {
                Type type = typeof (Tools);
                FieldInfo field = type.GetField ("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
                return ((bool) field.GetValue (null));
            }
            set {
                Type type = typeof (Tools);
                FieldInfo field = type.GetField ("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
                field.SetValue (null, value);
            }
        }
        #endif
    }   
}