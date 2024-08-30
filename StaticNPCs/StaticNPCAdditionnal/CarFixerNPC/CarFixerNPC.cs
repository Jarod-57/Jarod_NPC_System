using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CarFixerNPC : MonoBehaviour
{
    private NPCAnimationController animationController;
    private NavMeshAgent agent;
    private Vector3 originalPosition;
    private InputManager inputManager;
    private bool isReturning = false;
    private GameObject player;
    private GameObject playerCar;

    public GameObject carPrefab;  
    public float interactionDistance = 3f;  


    void Start()
    {
        animationController = GetComponent<NPCAnimationController>();
        agent = GetComponent<NavMeshAgent>();
        originalPosition = transform.position;
        player = GameObject.FindWithTag("Player");
        playerCar = GameObject.FindWithTag("PlayerCar"); 
        inputManager = player.GetComponent<InputManager>();

        GunshotEventManager.OnGunshotFired += OnGunshotFired;

        StartMechanicBehavior();
    }

    private void OnDestroy()
    {
        GunshotEventManager.OnGunshotFired -= OnGunshotFired;
    }

    private void Update()
    {
        CheckForPlayerInteraction();
    }

    private void StartMechanicBehavior()
    {
        // animationController.SetTrigger("StartRepairing");
    }

    private void OnGunshotFired(Vector3 gunshotPosition)
    {
        float distanceToGunshot = Vector3.Distance(transform.position, gunshotPosition);
        if (distanceToGunshot <= 70f)
        {
            StopCoroutine("ReturnToOriginalPositionAfterDelay"); 
            FleeFromGunshot(gunshotPosition);
        }
    }

    private void FleeFromGunshot(Vector3 gunshotPosition)
    {
        if (isReturning) return; 

        Vector3 fleeDirection = (transform.position - gunshotPosition).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * 70f;

        agent.speed *= 2; 
        if (agent) agent.SetDestination(fleeTarget);

        StartCoroutine(StopRunningAfterDistance(70f));
    }

    private IEnumerator StopRunningAfterDistance(float fleeDistance)
    {
        if (animationController != null) animationController.SetRunning(true);

        while (Vector3.Distance(transform.position, agent.destination) > fleeDistance) yield return null;

        if (animationController != null) animationController.SetRunning(false);

        StartCoroutine(ReturnToOriginalPositionAfterDelay(60f));
    }

    private IEnumerator ReturnToOriginalPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isReturning = true;
        agent.speed /= 2; 
        if (agent) agent.SetDestination(originalPosition); 

        if (animationController != null) animationController.SetRunning(true);

        while (Vector3.Distance(transform.position, originalPosition) > 0.5f) yield return null;

        if (animationController != null) animationController.SetRunning(false);

        isReturning = false;
    }

    private void CheckForPlayerInteraction()
    {
        if (player == null || playerCar == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= interactionDistance && inputManager.Talk) ReplacePlayerCar();
    }

    private void ReplacePlayerCar()
    {
        // if (playerCar == null || carPrefab == null) return;

        carPrefab = playerCar;

        Vector3 newCarPosition = player.transform.position - player.transform.forward * 5f;
        GameObject newCar = Instantiate(carPrefab, newCarPosition, Quaternion.identity);

        Destroy(playerCar);

        playerCar = newCar;
    }
}
