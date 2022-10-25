using UnityEngine;

public class HandsAnimation : MonoBehaviour
{
    private Animator handsAnimator;

    //controlling hands animation depends with action.
    private void Start()
    {
        handsAnimator = GetComponent<Animator>();
    }
    public void InteractAnimation()
    {
        handsAnimator.SetTrigger("Interact");
    }
    public void WalkingAnimation(bool state)
    {
        handsAnimator?.SetBool("isWalking", state);
    }
    public void BlackHoleAnimation()
    {
        handsAnimator.SetTrigger("BlackHole");
    }
    public void TelePullAnimation()
    {
        handsAnimator.SetTrigger("TelePull");
    }
    public void TelePullReleaseAnimation()
    {
        handsAnimator.SetTrigger("TelePullRelease");
    }
}
