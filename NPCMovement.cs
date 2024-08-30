using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NPCMovement : MonoBehaviour
{
    public Transform[] waypoints;
    private NavMeshAgent agent;
    private float minDistanceToNextWaypoint = 10f;
    private Vector3 originalPosition;
    private bool isFleeing = false;
    private float fleeRadius = 70f;
    private float originalSpeed;
    private Coroutine fleeCoroutine;
    private NPCAnimationController animationController;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        originalPosition = transform.position;
        SetNextDestination();

        animationController = GetComponent<NPCAnimationController>();

        GunshotEventManager.OnGunshotFired += OnGunshotFired;
    }

    void OnDestroy()
    {
        GunshotEventManager.OnGunshotFired -= OnGunshotFired;
    }

    void Update()
    {
        if (!isFleeing && !agent.pathPending && agent.remainingDistance < 0.5f) SetNextDestination();
    }

    void SetNextDestination()
    {
        if (waypoints.Length == 0) return;
        RemoveNullWaypoints();
        if (waypoints.Length == 0) return;

        Transform nextWaypoint = GetRandomWaypoint();
        while (Vector3.Distance(transform.position, nextWaypoint.position) < minDistanceToNextWaypoint) nextWaypoint = GetRandomWaypoint();

        agent.SetDestination(nextWaypoint.position);
    }

    Transform GetRandomWaypoint()
    {
        return waypoints[Random.Range(0, waypoints.Length)];
    }

    void RemoveNullWaypoints()
    {
        List<Transform> validWaypoints = new List<Transform>();

        foreach (Transform waypoint in waypoints) if (waypoint != null) validWaypoints.Add(waypoint);

        waypoints = validWaypoints.ToArray();
    }

    void OnGunshotFired(Vector3 gunshotPosition)
    {
        float distanceToGunshot = Vector3.Distance(transform.position, gunshotPosition);
        if (distanceToGunshot <= fleeRadius)
        {
            Vector3 fleeDirection = (transform.position - gunshotPosition).normalized;
            Vector3 fleeTarget = transform.position + fleeDirection * fleeRadius;

            if (agent != null)
            {
                agent.speed = originalSpeed * 2; 
                agent.SetDestination(fleeTarget);
                isFleeing = true;

                if (animationController != null) animationController.SetRunning(true);
                if (fleeCoroutine != null) StopCoroutine(fleeCoroutine);
                

                fleeCoroutine = StartCoroutine(ReturnToOriginalPositionAfterDelay(60f));
            }
        }
    }

    private IEnumerator ReturnToOriginalPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        agent.SetDestination(originalPosition);
        agent.speed = originalSpeed;
        isFleeing = false;

        if (animationController != null) animationController.SetRunning(false);
    }
}
