using System.Threading.Tasks;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private InteractiveAreaScript interactiveArea;

    [SerializeField] private Daylight mDaylight;

    private GameObject mMonoi = null;

    private void Start()
    {
        mMonoi = GameObject.FindGameObjectWithTag("Player");
    }

    async void Update()
    {
        if (interactiveArea.isUsing == false && interactiveArea.canIntercat == true && InputManager.Get().mPlayerInteract.Player.Open.triggered == true)
        {
            InputManager.Get().mPlayerInteract.Disable();
            InputManager.Get().mPlayerMovement.Disable();

            await Task.Delay(500);

            await mDaylight.ChangeDay();

            if (mMonoi != null)
                mMonoi.transform.rotation = Quaternion.Euler(0, -90, 0);

            await Task.Delay(100);

            interactiveArea.Quit();
        }
    }
}
