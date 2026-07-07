using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    [SerializeField] private Daylight mDaylight;

    [SerializeField] private Text mTextLoading;
    [SerializeField] public CanvasGroup mCanvasGroup;
    int mTextStep = 0;
    
    [SerializeField] private float mFadeTime = 0.5f;
    
    int mLoadingTasksAmount = 0;

    public static LoadingScript msInstance;

    void Awake()
    {
        msInstance = this;
        gameObject.SetActive(false);
    }

    public void StartLoading()
    {
        mLoadingTasksAmount += 1;

        if (mLoadingTasksAmount <= 1)
        {
            gameObject.SetActive(true);
            StartCoroutine(CoroutineLoading());
        }
    }

    public void StopLoading()
    {
        mLoadingTasksAmount -= 1;
    }

    IEnumerator CoroutineLoading()
    {
        mDaylight.mStopTime = true;

        while (mCanvasGroup.alpha < 1f)
        {
            mCanvasGroup.alpha += Time.deltaTime / mFadeTime;
            yield return null;
        }

        while (mLoadingTasksAmount > 0)
        {
            if (mTextStep >= 3)
                mTextStep = 0;
            else
                mTextStep += 1;

            mTextLoading.text = "Loading " + new string('.', mTextStep);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        while (mCanvasGroup.alpha > 0f)
        {
            mCanvasGroup.alpha -= Time.deltaTime / mFadeTime;
            yield return null;
        }

        mDaylight.mStopTime = false;
        gameObject.SetActive(false);
    }
}
