using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public Animator mPlayerAnimator;
    public void IsSitting()
    {
        mPlayerAnimator.SetBool("IsSitting", true);

        transform.position = new Vector3(-4.13f, 0.3f, 3.87f);
        mPlayerAnimator.CrossFade("Monoil_rig|Monoil_rig|Monoi_Sitting_Idle", 0f);
    }
}
