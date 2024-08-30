using UnityEngine;

public class NPCAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetRunning(bool isRunning)
    {
        if (animator != null) animator.SetBool("isNPCRunning", isRunning);
    }

    public void SetTrigger(string triggerName)
    {
        if (animator != null) animator.SetTrigger(triggerName);
    }
}
