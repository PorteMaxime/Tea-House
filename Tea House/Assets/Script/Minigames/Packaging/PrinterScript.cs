using UnityEngine;

public class PrinterScript : MonoBehaviour
{
    [SerializeField] private Input_PlayerInteract mInput;
    private InteractiveAreaScript mInteract;

    void Awake()
    {
        mInput = new Input_PlayerInteract();
        mInteract = transform.parent.GetComponentInChildren<InteractiveAreaScript>();
    }
    void OnEnable() => mInput.Enable();
    void OnDisable() => mInput.Disable();

    void Update()
    {
        if (mInteract.isUsing) UsingUpdate();
    }

    private void UsingUpdate()
    {

    }

    void PrintLabel()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
    }
}