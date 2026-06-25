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
    Quaternion targetRotation = Quaternion.Euler(0,90,0);
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

        initialPosition = mPlayer.transform.position;
        initialRotation = mPlayer.transform.rotation;

        GetComponent<BoxCollider>().enabled = false;
        float elapsedTime = 0;
        float duration = 0.5f;
        Vector3 positionOrigine = mPlayer.transform.position;
        Quaternion rotationOrigine = mPlayer.transform.rotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            mPlayer.transform.position = Vector3.Lerp(positionOrigine, targetPosition, elapsedTime / duration);
            mPlayer.transform.rotation = Quaternion.Lerp(rotationOrigine, targetRotation, elapsedTime / duration);
            yield return 0;
        }
        mPlayer.transform.position = targetPosition;
        mPlayer.transform.rotation = targetRotation;
        mPlayerAnimator.SetTrigger("SitDownTrigger");

        yield return new WaitForSeconds(1.8f);

        isSitting = true;
        mPlayerAnimator.SetBool("IsSitting", isSitting);
        mPlayer.transform.position = new Vector3(-4.13f, 0.3f, 3.87f);


        isBusy = false;
    }

    IEnumerator SitDownAnimation()
    {
        isBusy = true;

        isSitting = false;
        mPlayerAnimator.SetBool("IsSitting", isSitting);

        mPlayer.transform.position = targetPosition;

        yield return new WaitForSeconds(1.8f);

        float elapsedTime = 0;
        float duration = 0.5f;
        Vector3 positionOrigine = mPlayer.transform.position;
        //Quaternion rotationOrigine = mPlayer.transform.rotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            mPlayer.transform.position = Vector3.Lerp(positionOrigine, initialPosition, elapsedTime / duration);
            //mPlayer.transform.rotation = Quaternion.Lerp(rotationOrigine, initialRotation, elapsedTime / duration);
            yield return 0;
        }
        mPlayer.transform.position = initialPosition;
        //mPlayer.transform.rotation = initialRotation;

        GetComponent<BoxCollider>().enabled = true;
        isBusy = false;
    }
    IEnumerator mcMoveTo(Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0;
        Vector3 positionOrigine = transform.position;
        Quaternion rotationOrigine = transform.rotation;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(positionOrigine, targetPosition, elapsedTime / duration);
            transform.rotation = Quaternion.Lerp(rotationOrigine, targetRotation, elapsedTime / duration);
            yield return 0;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}
