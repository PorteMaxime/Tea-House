using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class AnimationEvent : MonoBehaviour
{
    public Animator mPlayerAnimator;
    public void IsSitting()
    {
        Debug.Log("End IsSitting");
        mPlayerAnimator.SetBool("IsSitting", true);

        transform.position = new Vector3(-4.13f, 0.3f, 3.87f);
        mPlayerAnimator.CrossFade("Monoil_rig|Monoil_rig|Monoi_Sitting_Idle", 0f);
    }
}
