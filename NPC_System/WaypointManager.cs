using UnityEngine;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour
{
    public bool showGizmos = true;
    public float gizmoSize = 0.5f;
    public Color gizmoColor = Color.yellow;

    public List<Transform> waypoints = new List<Transform>();
    private NPCSpawner npcSpawner;

    void Start()
    {
        npcSpawner = FindObjectOfType<NPCSpawner>();
        if (npcSpawner != null)
        {
            npcSpawner.waypoints = waypoints;
        }
        UpdateWaypoints();
    }

    public void UpdateWaypoints()
    {
        waypoints.Clear();
        foreach (Transform child in transform)
        {
            if (child != null)
            {
                waypoints.Add(child);
            }
        }

        if (npcSpawner != null)
        {
            npcSpawner.waypoints = new List<Transform>(waypoints); // Mise à jour directe des waypoints
        }
    }

    public void AddWaypoint(Transform waypoint)
    {
        waypoints.Add(waypoint);
        UpdateWaypoints();
    }

    public void RemoveWaypoint(Transform waypoint)
    {
        if (waypoints.Contains(waypoint))
        {
            waypoints.Remove(waypoint);
            DestroyImmediate(waypoint.gameObject);
            UpdateWaypoints();
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = gizmoColor;
        foreach (Transform waypoint in waypoints)
        {
            if (waypoint != null)
            {
                Gizmos.DrawSphere(waypoint.position, gizmoSize);
            }
        }

        for (int i = 1; i < waypoints.Count; i++)
        {
            if (waypoints[i - 1] != null && waypoints[i] != null)
            {
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
            }
        }
    }
}
