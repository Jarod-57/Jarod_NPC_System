using UnityEngine;
using UnityEngine.AI;

public class NPCConfigurator : MonoBehaviour
{
    public Transform[] waypoints; 


    public void ConfigureNPC(GameObject npc)
    {
        
        NavMeshAgent agent = npc.AddComponent<NavMeshAgent>();
        agent.speed = 3.5f;
        agent.acceleration = 8f;
        agent.angularSpeed = 120f;


        NPCMovement movementScript = npc.AddComponent<NPCMovement>();
        movementScript.waypoints = waypoints;


        Animator animator = npc.AddComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AnimatorControllers/NPCAnimator");


    }
}
