using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;
using System.Collections;

public class NPCSpawner : MonoBehaviour
{
    public RuntimeAnimatorController npcAnimatorStateController;
    private DynamicRagdollCreator ragdollCreator;
    private NPCConfigurator npcConfigurator;

    public GameObject[] baseNpcPrefabs;
    [HideInInspector] public List<Transform> spawnPoints;
    [HideInInspector] public List<Transform> waypoints;
    public int npcCount = 10;
    public float respawnCheckInterval = 5f;
    public float playerProximityRadius = 200f;

    public float minNpcSpeed = 1f;
    public float maxNpcSpeed = 1.6f;    

    private Transform playerTransform;
    private List<GameObject> activeNPCs = new List<GameObject>();



    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;

        WaypointManager waypointManager = FindObjectOfType<WaypointManager>();
        if (waypointManager != null) waypoints = waypointManager.waypoints;

        SpawnpointManager spawnpointManager = FindObjectOfType<SpawnpointManager>();
        if (spawnpointManager != null) spawnPoints = spawnpointManager.spawnpoints;


        npcConfigurator = gameObject.AddComponent<NPCConfigurator>();
        npcConfigurator.waypoints = waypoints.ToArray();

        ragdollCreator = gameObject.AddComponent<DynamicRagdollCreator>();

        for (int i = 0; i < npcCount; i++) SpawnAndConfigureNPC();


        InvokeRepeating("CheckAndRespawnNPCs", respawnCheckInterval, respawnCheckInterval);
    }

    private void SpawnAndConfigureNPC()
    {
        int randomSpawnIndex = Random.Range(0, spawnPoints.Count);
        int randomPrefabIndex = Random.Range(0, baseNpcPrefabs.Length);

        GameObject npc = Instantiate(baseNpcPrefabs[randomPrefabIndex], spawnPoints[randomSpawnIndex].position, Quaternion.identity);
        NPCMovement npcMovement = npc.AddComponent<NPCMovement>();
        npcMovement.waypoints = waypoints.ToArray();

        NavMeshAgent agent = npc.AddComponent<NavMeshAgent>();
        agent.speed = Random.Range(minNpcSpeed, maxNpcSpeed);

        activeNPCs.Add(npc);

        CapsuleCollider capsuleCollider = npc.AddComponent<CapsuleCollider>();
        capsuleCollider.height = 2f;
        capsuleCollider.radius = 0.35f;
        capsuleCollider.center = new Vector3(0, 1.0f, 0);

        npc.AddComponent<Animator>();
        Animator animator = npc.GetComponent<Animator>(); 
        animator.runtimeAnimatorController = npcAnimatorStateController;

        npc.tag = "NPC";
        ragdollCreator.CreateRagdoll(npc);

        // "Conflict" when disabling Ragdoll cause only 1 NPC Spawn
        // DynamicRagdollController ragdollController = npc.AddComponent<DynamicRagdollController>();
        // ragdollController.DisableRagdoll();


        npc.AddComponent<ShootableNPC>();
    }

    private void CheckAndRespawnNPCs()
    {
        activeNPCs.RemoveAll(npc => npc == null);

        int npcsToSpawn = npcCount - activeNPCs.Count;

        for (int i = 0; i < npcsToSpawn; i++)
        {
            bool playerIsNearby = false;

            foreach (Transform spawnPoint in spawnPoints)
            {
                if (Vector3.Distance(playerTransform.position, spawnPoint.position) <= playerProximityRadius)
                {
                    playerIsNearby = true;
                    break;
                }
            }

            if (!playerIsNearby) SpawnAndConfigureNPC();
        }

    }

    Avatar GetAvatarFromPrefab(GameObject prefab) {
        Animator prefabAnimator = prefab.GetComponent<Animator>();

        if (prefabAnimator != null) return prefabAnimator.avatar;
        else return null;
    }


    public void UpdateWaypoints(List<Transform> newWaypoints)
    {
        waypoints = newWaypoints;
    }

    public void UpdateSpawnpoints(List<Transform> newSpawnpoints)
    {
        spawnPoints = newSpawnpoints;
    }
}
