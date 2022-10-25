using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    private Animator animator;

    //attacking animation
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void WalkingAnimation(bool state)
    {
        animator.SetBool("isWalking", state);
    }
    public void AttackAnimation(bool state)
    {
        animator.SetBool("swingAttack", state);
    }
}
