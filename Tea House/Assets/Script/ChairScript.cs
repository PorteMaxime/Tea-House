using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ChairScript : MonoBehaviour
{
    public InteractiveAreaScript interactiveArea;

    bool isSitting = false;
    float sitIdleTimer = 0f;
    public float sitExtraDelay = 6f;
    GameObject mPlayer;
    bool isBusy = false;

    Vector3 initialPosition = Vector3.zero;
    Quaternion initialRotation = Quaternion.identity;

    Vector3 targetPosition = new Vector3(-3.75f, 0.3f, 3.87f);
    Quaternion targetRotation = Quaternion.Euler(0, 90, 0);
    Animator mPlayerAnimator;
    void Start()
    {
        mPlayer = GameObject.FindGameObjectWithTag("Player");
        interactiveArea.interactWithCamera = false;
        mPlayerAnimator = mPlayer.GetComponent<Animator>();
    }

    void Update()
    {
        if (interactiveArea.canIntercat == true && InputManager.Get().mPlayerInteract.Player.Open.triggered == true && isSitting == true && isBusy == false)
        {
            StartCoroutine(SitDownAnimation());
        }
        if (interactiveArea.canIntercat == true && InputManager.Get().mPlayerInteract.Player.Open.triggered == true && isSitting == false && isBusy == false)
        {
            StartCoroutine(SitAnimation());
        }
        if (isSitting == true)
        {
            sitIdleTimer += Time.deltaTime;
            if (sitIdleTimer > sitExtraDelay)
            {
                sitIdleTimer = -3.6f;
                mPlayerAnimator.SetTrigger("SittingExtraTrigger");
            }
        }
    }

    IEnumerator SitAnimation()
    {
        isBusy = true;

        GetComponent<BoxCollider>().enabled = false;

        float elapsedTime = 0;
        float duration = 0.5f;

        Vector3 positionOrigine = initialPosition = mPlayer.transform.position;
        Quaternion rotationOrigine = initialRotation = mPlayer.transform.rotation;
        mPlayerAnimator.SetBool("IsWalking", true);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            mPlayer.transform.position = Vector3.Lerp(positionOrigine, targetPosition, elapsedTime / duration);
            mPlayer.transform.rotation = Quaternion.Lerp(rotationOrigine, targetRotation, elapsedTime / duration);
            yield return 0;
        }
        mPlayer.transform.position = targetPosition;
        mPlayer.transform.rotation = targetRotation;

        mPlayerAnimator.SetBool("IsWalking", false);

        mPlayerAnimator.SetTrigger("SitDownTrigger");
        AnimatorStateInfo stateInfo = mPlayerAnimator.GetCurrentAnimatorStateInfo(0);

        isSitting = true;

        isBusy = false;
    }

    IEnumerator SitDownAnimation()
    {
        isBusy = true;

        isSitting = false;
        mPlayerAnimator.SetBool("IsSitting", isSitting);

        mPlayer.transform.position = targetPosition;
        mPlayerAnimator.CrossFade("Monoil_rig|Monoil_rig|Monoil_Sit 0", 0f);

        yield return new WaitForSeconds(1.8f);

        float elapsedTime = 0;
        float duration = 0.5f;
        Vector3 positionOrigine = mPlayer.transform.position;

        mPlayerAnimator.SetBool("IsWalking", true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            mPlayer.transform.position = Vector3.Lerp(positionOrigine, initialPosition, elapsedTime / duration);
            //mPlayer.transform.rotation = Quaternion.Lerp(rotationOrigine, initialRotation, elapsedTime / duration);
            yield return 0;
        }
        mPlayer.transform.position = initialPosition;

        mPlayerAnimator.SetBool("IsWalking", false);

        GetComponent<BoxCollider>().enabled = true;
        isBusy = false;
    }
}