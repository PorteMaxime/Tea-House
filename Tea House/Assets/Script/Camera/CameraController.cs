using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour
{
    private Input_PlayerMovement mInput;

    [SerializeField] private float mRotationSpeed;
    private float mCurrrentRotation = 0f;

    void Awake() => mInput = new Input_PlayerMovement();
    void OnEnable() => mInput.Enable();
    void OnDisable() => mInput.Disable();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mInput.Camera.Rotation.triggered)
        {
            float axis = mInput.Camera.Rotation.ReadValue<float>();
            float rotationAmount = axis * mRotationSpeed;
            transform.parent.Rotate(0, rotationAmount, 0);
        }
    }
}
