using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaypointManager))]
public class WaypointEditor : Editor
{
    private WaypointManager waypointManager;
    private SerializedProperty waypointsProperty;
    private bool isAddingWaypoints;
    private Vector3 previewWaypointPosition;

    private void OnEnable()
    {
        waypointManager = (WaypointManager)target;
        waypointsProperty = serializedObject.FindProperty("waypoints");
    }

    private void OnSceneGUI()
    {
        Event e = Event.current;

        if (e.shift && e.type == EventType.MouseMove)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                previewWaypointPosition = hit.point;
                SceneView.RepaintAll();
            }
        }

        if (e.shift && e.type == EventType.MouseDown && e.button == 0)
        {
            AddWaypoint(previewWaypointPosition);
            e.Use();
        }

        if (e.shift && e.alt && e.type == EventType.MouseDown && e.button == 0)
        {
            RemoveWaypointAtMousePosition();
            e.Use();
        }

        if (waypointManager.showGizmos)
        {
            DrawWaypoints();
            if (e.shift && !e.alt)
            {
                DrawPreviewWaypoint(previewWaypointPosition);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Toggle Gizmos"))
        {
            waypointManager.showGizmos = !waypointManager.showGizmos;
        }
    }

    private void AddWaypoint(Vector3 position)
    {
        GameObject waypoint = new GameObject("Waypoint");
        waypoint.transform.position = position;
        waypoint.transform.SetParent(waypointManager.transform);
        waypointManager.UpdateWaypoints();
        UpdateNPCSpawnerWaypoints();

        // Rétablir la sélection sur le WaypointManager
        Selection.activeGameObject = waypointManager.gameObject;
    }

    private void RemoveWaypointAtMousePosition()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform waypoint = hit.transform;
            if (waypoint != null && waypointManager.transform.IsChildOf(waypoint))
            {
                waypointManager.RemoveWaypoint(waypoint);
                UpdateNPCSpawnerWaypoints();

                // Rétablir la sélection sur le WaypointManager
                Selection.activeGameObject = waypointManager.gameObject;
            }
        }
    }

    private void UpdateNPCSpawnerWaypoints()
    {
        if (waypointManager == null) return;

        NPCSpawner npcSpawner = waypointManager.GetComponent<NPCSpawner>();
        if (npcSpawner == null) return;

        serializedObject.Update();
        waypointsProperty.ClearArray();

        for (int i = 0; i < waypointManager.transform.childCount; i++)
        {
            waypointsProperty.InsertArrayElementAtIndex(i);
            waypointsProperty.GetArrayElementAtIndex(i).objectReferenceValue = waypointManager.transform.GetChild(i);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPreviewWaypoint(Vector3 position)
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(position, Vector3.up, waypointManager.gizmoSize);

        Transform[] waypoints = waypointManager.GetComponentsInChildren<Transform>();
        if (waypoints.Length > 1)
        {
            Handles.DrawLine(waypoints[waypoints.Length - 1].position, position);
        }
    }

    private void DrawWaypoints()
    {
        Transform[] waypoints = waypointManager.GetComponentsInChildren<Transform>();

        Handles.color = waypointManager.gizmoColor;
        for (int i = 1; i < waypoints.Length; i++)
        {
            Handles.SphereHandleCap(0, waypoints[i].position, Quaternion.identity, waypointManager.gizmoSize, EventType.Repaint);

            if (i > 1)
            {
                Handles.DrawLine(waypoints[i - 1].position, waypoints[i].position);
            }
        }
    }
}
