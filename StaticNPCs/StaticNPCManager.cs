using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StaticNPCManager : MonoBehaviour
{
    public float checkInterval = 6.0f; 
    public float respawnDistance = 30.0f; 

    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        StartCoroutine(CheckAndRespawnNPCs());
    }

    private IEnumerator CheckAndRespawnNPCs()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            GameObject[] allNPCs = FindGameObjectsWithLayer(LayerMask.NameToLayer("StaticNPC"));

            foreach (GameObject npc in allNPCs)
            {

                if (!npc.activeSelf && !npc.activeInHierarchy)
                {
                    float distance = Vector3.Distance(npc.transform.position, player.transform.position);
                    
                    if (distance > respawnDistance)
                    {
                        npc.SetActive(true);

                        MonoBehaviour[] scripts = npc.GetComponents<MonoBehaviour>();
                        foreach (var script in scripts) script.enabled = true;

                        Collider npcCollider = npc.GetComponent<Collider>();
                        if (npcCollider != null) npcCollider.enabled = true;

                        Animator animator = npc.GetComponent<Animator>();
                        if (animator != null) animator.enabled = true;

                        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
                        if (agent != null) agent.enabled = true;

                        var shootableNPC = npc.GetComponent<ShootableNPC>();
                        if (shootableNPC != null) {
                            shootableNPC.isDead = false;
                            shootableNPC.currentHealth = 100;
                        }
                    }
                }
            }
        }
    }

    public static GameObject[] FindGameObjectsWithLayer(int layer)
    {
        List<GameObject> foundObjects = new List<GameObject>();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);

        foreach (GameObject obj in allObjects) if (obj.layer == layer) foundObjects.Add(obj);

        return foundObjects.ToArray();
    }
}
