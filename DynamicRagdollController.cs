using UnityEngine;
using UnityEngine.AI;

public class DynamicRagdollController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        if (animator != null)
        {
            animator.enabled = false;
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        NPCMovement npcMovement = GetComponent<NPCMovement>();
        if (npcMovement != null)
        {
            npcMovement.enabled = false;
        }
    }

    public void DisableRagdoll()
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = true;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        if (animator != null)
        {
            animator.enabled = true;
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = true;
        }

        NPCMovement npcMovement = GetComponent<NPCMovement>();
        if (npcMovement != null)
        {
            npcMovement.enabled = true;
        }
    }
}
