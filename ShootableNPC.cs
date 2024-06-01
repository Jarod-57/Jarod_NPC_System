using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShootableNPC : MonoBehaviour
{
    public int currentHealth = 100;
    public GameObject player;
    Animator animator;
    Collider npcCollider;
    NavMeshAgent agent;
    public bool isDead = false;
    private float checkDistanceInterval = 1.0f; 
    private float destroyDistance = 200.0f; 

    void Start()
    {
        animator = GetComponent<Animator>();
        npcCollider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindWithTag("Player");
        StartCoroutine(CheckDistance());
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0) 
        {
            isDead = true;
            animator.enabled = false;
            npcCollider.enabled = false;
            if (agent != null) agent.enabled = false;
            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
            foreach (var script in scripts)
            {
                if (script != this) script.enabled = false;
            }
            
        }
    }

    private IEnumerator CheckDistance()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkDistanceInterval);

            if (isDead && player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);

                if (distance > destroyDistance) Destroy(gameObject);
            }
        }

    }
}
