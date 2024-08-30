using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnpointManager))]
public class SpawnpointEditor : Editor
{
    private SpawnpointManager spawnpointManager;
    private SerializedProperty spawnpointsProperty;
    private bool isAddingSpawnpoints;
    private Vector3 previewSpawnpointPosition;

    private void OnEnable()
    {
        spawnpointManager = (SpawnpointManager)target;
        spawnpointsProperty = serializedObject.FindProperty("spawnpoints");
    }

    private void OnSceneGUI()
    {
        Event e = Event.current;

        if (e.shift && e.type == EventType.MouseMove)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                previewSpawnpointPosition = hit.point;
                SceneView.RepaintAll();
            }
        }

        if (e.shift && e.type == EventType.MouseDown && e.button == 0)
        {
            AddSpawnpoint(previewSpawnpointPosition);
            e.Use();
        }

        if (e.shift && e.alt && e.type == EventType.MouseDown && e.button == 0)
        {
            RemoveSpawnpointAtMousePosition();
            e.Use();
        }

        if (spawnpointManager.showGizmos)
        {
            DrawSpawnpoints();
            if (e.shift && !e.alt)
            {
                DrawPreviewSpawnpoint(previewSpawnpointPosition);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Toggle Gizmos"))
        {
            spawnpointManager.showGizmos = !spawnpointManager.showGizmos;
        }
    }

    private void AddSpawnpoint(Vector3 position)
    {
        GameObject spawnpoint = new GameObject("Spawnpoint");
        spawnpoint.transform.position = position;
        spawnpoint.transform.SetParent(spawnpointManager.transform);
        spawnpointManager.UpdateSpawnpoints();

        // Rétablir la sélection sur le SpawnpointManager
        Selection.activeGameObject = spawnpointManager.gameObject;
    }

    private void RemoveSpawnpointAtMousePosition()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform spawnpoint = hit.transform;
            if (spawnpoint != null && spawnpointManager.transform.IsChildOf(spawnpoint))
            {
                spawnpointManager.RemoveSpawnpoint(spawnpoint);
            }
        }
    }

    private void UpdateNPCSpawnerSpawnpoints()
    {
        if (spawnpointManager == null) return;

        NPCSpawner npcSpawner = spawnpointManager.GetComponent<NPCSpawner>();
        if (npcSpawner == null) return;

        serializedObject.Update();
        spawnpointsProperty.ClearArray();

        for (int i = 0; i < spawnpointManager.transform.childCount; i++)
        {
            spawnpointsProperty.InsertArrayElementAtIndex(i);
            spawnpointsProperty.GetArrayElementAtIndex(i).objectReferenceValue = spawnpointManager.transform.GetChild(i);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPreviewSpawnpoint(Vector3 position)
    {
        Handles.color = Color.blue;
        Handles.DrawWireDisc(position, Vector3.up, spawnpointManager.gizmoSize);
    }

    private void DrawSpawnpoints()
    {
        Transform[] spawnpoints = spawnpointManager.GetComponentsInChildren<Transform>();

        Handles.color = spawnpointManager.gizmoColor;
        for (int i = 1; i < spawnpoints.Length; i++)
        {
            Handles.SphereHandleCap(0, spawnpoints[i].position, Quaternion.identity, spawnpointManager.gizmoSize, EventType.Repaint);
        }
    }
}
