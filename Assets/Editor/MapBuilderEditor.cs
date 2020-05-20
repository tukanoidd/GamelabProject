#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEditor;
using UnityEngine;
using Plane = DataTypes.Plane;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderEditor : Editor
{
    private MapBuilder _mapBuilder;

    private void OnEnable()
    {
        _mapBuilder = (MapBuilder) target;
    }

    public override void OnInspectorGUI()
    {
        if (_mapBuilder)
        {
            if (GUILayout.Button("Center Position"))
            {
                HelperMethods.CenterPosition(_mapBuilder.transform);
            }

            if (GUILayout.Button("Add Map Part Builder"))
            {
                Selection.activeGameObject = _mapBuilder.AddMapPartBuilder();
            }

            if (GUILayout.Button("Rename Children In Hierarchy Order"))
            {
                RenameBlocksHierarchyOrder();
            }

            if (_mapBuilder.PathFindingMapsDataExists)
            {
                Plane[] availablePlanes = _mapBuilder.AvailablePlanes;
                if (availablePlanes.Length > 0)
                {
                    Plane selectedPlane = availablePlanes.Contains(_mapBuilder.showMapGravitationalPlane.plane)
                        ? _mapBuilder.showMapGravitationalPlane.plane
                        : availablePlanes[0];

                    GUILayout.Label("Map To Show");
                    GUILayout.BeginVertical();
                    GUILayout.Label("Gravitational Plane");

                    _mapBuilder.showMapGravitationalPlane.plane = availablePlanes[
                        EditorGUILayout.Popup(
                            "Plane",
                            Array.IndexOf(availablePlanes, selectedPlane),
                            availablePlanes.Select(plane => plane.ToString()).ToArray()
                        )
                    ];

                    PlaneSide[] availablePlaneSides = _mapBuilder.AvailablePlaneSides(_mapBuilder.showMapGravitationalPlane.plane);

                    if (availablePlaneSides.Length > 0)
                    {
                        PlaneSide selectedPlaneSide = availablePlaneSides.Contains(_mapBuilder.showMapGravitationalPlane.planeSide)
                            ? _mapBuilder.showMapGravitationalPlane.planeSide
                            : availablePlaneSides[0];

                        _mapBuilder.showMapGravitationalPlane.planeSide = availablePlaneSides[
                            EditorGUILayout.Popup(
                                "Plane Side",
                                Array.IndexOf(availablePlaneSides, selectedPlaneSide),
                                availablePlaneSides.Select(planeSide => planeSide.ToString()).ToArray()
                            )
                        ];

                        if (_mapBuilder.showMapGravitationalPlane != null)
                        {
                            if (!_mapBuilder.MapRepresentationExists)
                            {
                                if (GUILayout.Button("Show Map Representation"))
                                {
                                    _mapBuilder.ShowMap();
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Remove Map Representation"))
                                {
                                    _mapBuilder.RemoveMap();
                                }
                            }
                        }
                    }
                    
                    GUILayout.EndVertical();
                }
            }
        }

        DrawDefaultInspector();
    }

    private void RenameBlocksHierarchyOrder()
    {
        MapPartBuilder[] mapPartBuilders = FindObjectsOfType<MapPartBuilder>()
            .OrderBy(mapPartBuider => mapPartBuider.transform.GetSiblingIndex()).ToArray();

        if (mapPartBuilders.Length > 0)
        {
            List<Block> blocks = new List<Block>();

            for (int i = 0; i < mapPartBuilders.Length; i++)
            {
                mapPartBuilders[i].name = "MapPartBuilder " + (i + 1);

                Block[] blocksinMapBuider = mapPartBuilders[i].GetComponentsInChildren<Block>()
                    .OrderBy(block => block.transform.GetSiblingIndex()).ToArray();
                
                blocks.AddRange(blocksinMapBuider);
            }

            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i].name = "BuildingBlock " + (i + 1);
            }
        }
    }
}
#endif