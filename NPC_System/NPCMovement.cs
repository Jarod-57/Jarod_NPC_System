using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPCMovement : MonoBehaviour
{
    public Transform[] waypoints;
    private NavMeshAgent agent;
    private float minDistanceToNextWaypoint = 10f; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNextDestination();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SetNextDestination();
        }
    }

    void SetNextDestination()
    {
        if (waypoints.Length == 0)
            return;

        RemoveNullWaypoints(); 

        if (waypoints.Length == 0)
            return;

        Transform nextWaypoint = GetRandomWaypoint();
        while (Vector3.Distance(transform.position, nextWaypoint.position) < minDistanceToNextWaypoint)
        {
            nextWaypoint = GetRandomWaypoint();
        }

        agent.SetDestination(nextWaypoint.position);
    }

    Transform GetRandomWaypoint()
    {
        return waypoints[Random.Range(0, waypoints.Length)];
    }

    void RemoveNullWaypoints()
    {
        List<Transform> validWaypoints = new List<Transform>();
        foreach (Transform waypoint in waypoints)
        {
            if (waypoint != null)
            {
                validWaypoints.Add(waypoint);
            }
        }
        waypoints = validWaypoints.ToArray();
    }
}
