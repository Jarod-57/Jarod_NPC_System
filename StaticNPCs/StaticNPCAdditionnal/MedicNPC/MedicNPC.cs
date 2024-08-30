using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MedicNPC : MonoBehaviour
{
    private NPCAnimationController animationController;
    private NavMeshAgent agent;
    private Vector3 originalPosition;
    private InputManager inputManager;
    private bool isReturning = false;
    private GameObject player;
    private PlayerStats playerStats;

    public float interactionDistance = 3f;

    void Start()
    {
        animationController = GetComponent<NPCAnimationController>();
        agent = GetComponent<NavMeshAgent>();
        originalPosition = transform.position;
        player = GameObject.FindWithTag("Player");
        inputManager = player.GetComponent<InputManager>();
        playerStats = player.GetComponent<PlayerStats>();

        GunshotEventManager.OnGunshotFired += OnGunshotFired;

        StartMedicBehavior();
    }

    private void OnDestroy()
    {
        GunshotEventManager.OnGunshotFired -= OnGunshotFired;
    }

    private void Update()
    {
        CheckForPlayerInteraction();
    }

    private void StartMedicBehavior()
    {
        // animationController.SetTrigger("StartHealing");
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
        if (player == null || playerStats == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= interactionDistance && inputManager.Talk && playerStats.isDead)
        {
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        playerStats.RestartGame(); 
        playerStats.currentHealth = playerStats.maxHealth;
        playerStats.UpdateHealthBarFill();

        if (!isReturning)
        {
            StartCoroutine(ReturnToOriginalPositionAfterDelay(5f)); 
        }
    }
}
