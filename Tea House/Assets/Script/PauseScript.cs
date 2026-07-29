using TMPro;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    [SerializeField] private MainPanel mMainPanel;
    [SerializeField] private Daylight mDaylight;

    [SerializeField] private TextMeshProUGUI mTime;

    private void OnEnable()
    {
        mTime.text = mMainPanel.mTimer.text;

        mMainPanel.gameObject.SetActive(false);
        mDaylight.mStopTime = true;

        InputManager.Get().mPlayerInteract.Disable();
        InputManager.Get().mPlayerMovement.Disable();
    }

    private void OnDisable()
    {
        mMainPanel.gameObject.SetActive(true);
        mDaylight.mStopTime = false;
    }

    public void ReturnToGame()
    {
        this.gameObject.SetActive(false);
        
        InputManager.Get().mPlayerInteract.Enable();
        InputManager.Get().mPlayerMovement.Enable();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
