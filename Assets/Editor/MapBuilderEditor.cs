#if UNITY_EDITOR
using System;
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

            if (_mapBuilder.PathFindingMapsDataExists)
            {
                Plane[] availablePlanes = _mapBuilder.AvailablePlanes;
                if (availablePlanes.Length > 0)
                {
                    Plane selectedPlane = availablePlanes.Contains(_mapBuilder.showMap.plane)
                        ? _mapBuilder.showMap.plane
                        : availablePlanes[0];

                    GUILayout.Label("Map To Show");
                    GUILayout.BeginVertical();
                    GUILayout.Label("Gravitational Plane");

                    _mapBuilder.showMap.plane = availablePlanes[
                        EditorGUILayout.Popup(
                            "Plane",
                            Array.IndexOf(availablePlanes, selectedPlane),
                            availablePlanes.Select(plane => plane.ToString()).ToArray()
                        )
                    ];

                    PlaneSide[] availablePlaneSides = _mapBuilder.AvailablePlaneSides(_mapBuilder.showMap.plane);

                    if (availablePlaneSides.Length > 0)
                    {
                        PlaneSide selectedPlaneSide = availablePlaneSides.Contains(_mapBuilder.showMap.planeSide)
                            ? _mapBuilder.showMap.planeSide
                            : availablePlaneSides[0];

                        _mapBuilder.showMap.planeSide = availablePlaneSides[
                            EditorGUILayout.Popup(
                                "Plane Side",
                                Array.IndexOf(availablePlaneSides, selectedPlaneSide),
                                availablePlaneSides.Select(planeSide => planeSide.ToString()).ToArray()
                            )
                        ];

                        if (_mapBuilder.showMap != null)
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
}
#endif