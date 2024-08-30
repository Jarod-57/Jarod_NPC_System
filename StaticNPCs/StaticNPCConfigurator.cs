using UnityEngine;
using UnityEngine.AI;

public class StaticNPCConfigurator : MonoBehaviour
{
    public RuntimeAnimatorController defaultAnimatorController;
    public RuntimeAnimatorController carFixerAnimatorController;
    public RuntimeAnimatorController medicsAnimatorController;
    public NPCType npcType;

    private DynamicRagdollCreator ragdollCreator;
    private StaticNPCManager npcManager;

    private void Start()
    {
        AddCommonComponents();
        ConfigureNPCByType();
        AdjustColliderAndAgent();
    }

    private void AddCommonComponents()
    {
        npcManager = FindObjectOfType<StaticNPCManager>();

        if (gameObject.layer != LayerMask.NameToLayer("StaticNPC")) gameObject.layer = LayerMask.NameToLayer("StaticNPC");
        // if (npcManager != null) npcManager.RegisterNPC(gameObject, npcType, transform.position);
        

        if (GetComponent<NavMeshAgent>() == null) gameObject.AddComponent<NavMeshAgent>();

        if (GetComponent<CapsuleCollider>() == null)
        {
            CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.height = 2f;
            capsuleCollider.radius = 0.35f;
            capsuleCollider.center = new Vector3(0, 1.0f, 0);
        }

        if (GetComponent<Animator>() == null) gameObject.AddComponent<Animator>();
        if (GetComponent<ShootableNPC>() == null) gameObject.AddComponent<ShootableNPC>();
        if (GetComponent<NPCAnimationController>() == null) gameObject.AddComponent<NPCAnimationController>();

        ragdollCreator = gameObject.AddComponent<DynamicRagdollCreator>();
        ragdollCreator.CreateRagdoll(gameObject); 
    }

    private void ConfigureNPCByType()
    {
        Animator animator = GetComponent<Animator>();

        switch (npcType)
        {
            case NPCType.Mechanic:
                animator.runtimeAnimatorController = carFixerAnimatorController;
                gameObject.AddComponent<CarFixerNPC>();
                break;
            case NPCType.Medics:
                animator.runtimeAnimatorController = medicsAnimatorController;
                gameObject.AddComponent<MedicNPC>();
                break;
            default:
                animator.runtimeAnimatorController = defaultAnimatorController;
                break;
        }
    }

    private void AdjustColliderAndAgent()
    {
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            capsuleCollider.center = new Vector3(0, 0.9f, 0);  
            capsuleCollider.height = 2f;  
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.baseOffset = -0.1f;  
    }
}

public enum NPCType
{
    Mechanic,
    Medics,
}


