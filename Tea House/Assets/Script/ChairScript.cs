using UnityEngine;

public class ChairScript : MonoBehaviour
{
    public InteractiveAreaScript interactiveArea;

    bool isSitting = false;
    float sitIdleTimer = 0f;
    public float sitExtraDelay = 6f;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(interactiveArea.canIntercat == true && InputManager.Get().mPlayerInteract.Player.Select.IsPressed() == true)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
