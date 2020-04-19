using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapPartBuilder))]
    public class MapPartBuilderEditor : Editor
    {
        private MapPartBuilder _mapPartBuilder;

        private void OnEnable()
        {
            _mapPartBuilder = (MapPartBuilder) target;

            if (_mapPartBuilder)
            {
                if (!_mapPartBuilder.blockPrefab)
                {
                    _mapPartBuilder.blockPrefab = Resources.Load<GameObject>("Prefabs/BuildingBlockPrefab");
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (_mapPartBuilder)
            {
                Transform mapPartBuilderTransform = _mapPartBuilder.transform;
                Vector3 mapPartBuilderPosition = mapPartBuilderTransform.position;
                
                if (GUILayout.Button("Snap To 1x1x1 Grid"))
                {
                    HelperMethods.SnapToGrid(mapPartBuilderTransform);
                }

                if (GUILayout.Button("Snap To Block Sized Grid"))
                {
                    mapPartBuilderTransform.position = HelperMethods.SnapToBlockGrid(mapPartBuilderPosition);
                }

                if (GUILayout.Button("Create Starting Block"))
                {
                    _mapPartBuilder.CreateStartingBlock();
                }
            }

            DrawDefaultInspector();
        }
    }