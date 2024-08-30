using UnityEngine;
using System.Collections.Generic;

public class SpawnpointManager : MonoBehaviour
{
    public bool showGizmos = true;
    public float gizmoSize = 0.5f;
    public Color gizmoColor = Color.blue;

    public List<Transform> spawnpoints = new List<Transform>();
    private NPCSpawner npcSpawner;

    void Start()
    {
        npcSpawner = FindObjectOfType<NPCSpawner>();
        if (npcSpawner != null)
        {
            npcSpawner.spawnPoints = spawnpoints;
        }
        UpdateSpawnpoints();
    }

    public void UpdateSpawnpoints()
    {
        spawnpoints.Clear();
        foreach (Transform child in transform)
        {
            if (child != null)
            {
                spawnpoints.Add(child);
            }
        }

        if (npcSpawner != null)
        {
            npcSpawner.spawnPoints = new List<Transform>(spawnpoints); // Mise à jour directe des spawn points
        }
    }

    public void AddSpawnpoint(Transform spawnpoint)
    {
        spawnpoints.Add(spawnpoint);
        UpdateSpawnpoints();
    }

    public void RemoveSpawnpoint(Transform spawnpoint)
    {
        if (spawnpoints.Contains(spawnpoint))
        {
            spawnpoints.Remove(spawnpoint);
            DestroyImmediate(spawnpoint.gameObject);
            UpdateSpawnpoints();
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = gizmoColor;
        foreach (Transform spawnpoint in spawnpoints)
        {
            if (spawnpoint != null)
            {
                Gizmos.DrawSphere(spawnpoint.position, gizmoSize);
            }
        }
    }
}
