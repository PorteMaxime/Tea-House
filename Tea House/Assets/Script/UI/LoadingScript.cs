using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoadingScript : MonoBehaviour
{
    [SerializeField] private Daylight mDaylight;

    [SerializeField] public CanvasGroup mCanvasGroupBackground;
    [SerializeField] public CanvasGroup mCanvasGroupMonoi;
    [SerializeField] public CanvasGroup mScores;

    int mLoadingTasksAmount = 0;
    [SerializeField] float mMinimumLoadingTime = 2f;

    [SerializeField] private float mFadeTime = 0.5f;

    public static LoadingScript msInstance;

    public enum LoadingState
    {
        FadeInBackground,

        Scores,

        FadeInMonoi,

        FadeOutAll,
    }
    private LoadingState mLoadingState;
    public bool mShowScores = false;

    void Awake()
    {
        msInstance = this;
        gameObject.SetActive(false);
    }

    public void StartLoading(bool showScores, float baseBackgroundOpacity, float baseMonoiOpacity)
    {
        mLoadingTasksAmount += 1;

        if (mLoadingTasksAmount <= 1)
        {
            gameObject.SetActive(true);

            mCanvasGroupBackground.alpha = baseBackgroundOpacity;
            mCanvasGroupMonoi.alpha = baseMonoiOpacity;
            mShowScores = showScores;

            StartCoroutine(CoroutineLoading());
        }
        else if (showScores == true && mLoadingState < LoadingState.Scores)
            mShowScores = true;
    }

    public void StopLoading()
    {
        mLoadingTasksAmount -= 1;
    }

    IEnumerator CoroutineLoading()
    {
        mDaylight.mStopTime = true;

        InputManager.Get().mPlayerInteract.Disable();
        InputManager.Get().mPlayerMovement.Disable();

        mLoadingState = LoadingState.FadeInBackground;
        while (mCanvasGroupBackground.alpha < 1f)
        {
            mCanvasGroupBackground.alpha += Time.deltaTime / mFadeTime;
            yield return null;
        }

        mLoadingState = LoadingState.Scores;
        if (mShowScores == true)
        {
            mScores.alpha = 1f;

            while (!Keyboard.current.anyKey.wasPressedThisFrame)
                yield return null;
        }

        mLoadingState = LoadingState.FadeInMonoi;
        while (mCanvasGroupMonoi.alpha < 1f)
        {
            mCanvasGroupMonoi.alpha += Time.deltaTime / mFadeTime;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(mMinimumLoadingTime);

        while (mLoadingTasksAmount > 0)
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }

        mLoadingState = LoadingState.FadeOutAll;
        while (mCanvasGroupBackground.alpha > 0f)
        {
            mCanvasGroupBackground.alpha = mCanvasGroupMonoi.alpha -= Time.deltaTime / mFadeTime;
            yield return null;
        }

        mShowScores = false;
        mDaylight.mStopTime = false;
        gameObject.SetActive(false);

        InputManager.Get().mPlayerInteract.Enable();
        InputManager.Get().mPlayerMovement.Enable();
    }
}
