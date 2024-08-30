using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OldStaticNPCMovement : MonoBehaviour
{
    public String npcTag;

    private GameObject thisNPC;
    private List<String> allowedNPCTags;
    

    void Start()
    {
        thisNPC = GetComponent<GameObject>();
        ConfigureNPC(thisNPC);
    }

    public void ConfigureNPC(GameObject npc)
    {
        NavMeshAgent agent = npc.AddComponent<NavMeshAgent>();
        agent.speed = 3.5f;
        agent.acceleration = 8f;
        agent.angularSpeed = 120f;

        ChooseNPCType(npcTag);

        // Animator animator = npc.AddComponent<Animator>();
        // animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AnimatorControllers/NPCAnimator");
    }

    public void ChooseNPCType(String pNPCTag)
    {
        foreach (String i in allowedNPCTags) if (pNPCTag == i) thisNPC.tag = pNPCTag;
    }

    public void NPCTypeBehavior()
    {
        switch (thisNPC.tag) {
            case "NPC":
                ConfigureRegularStaticNPC();
                break;
            case "CarFixerNPC":
                ConfigureCarFixerNPC();
                break;
        }
    }

    public void ConfigureRegularStaticNPC()
    {

    }

    public void ConfigureCarFixerNPC()
    {
        BoxCollider boxCollider = thisNPC.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(7f, 2f, 7f);


        CapsuleCollider capsuleCollider = thisNPC.AddComponent<CapsuleCollider>();
        capsuleCollider.height = 2f;
        capsuleCollider.radius = 0.35f;
        capsuleCollider.center = new Vector3(0, 1.0f, 0);

        Animator animator = thisNPC.AddComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AnimatorControllers/CarFixerNPCAnimator");

        // thisNPC.AddComponent<CarFixerNPCACS>();
        thisNPC.AddComponent<ShootableNPC>();
    }
}
