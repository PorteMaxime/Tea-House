using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour
{
    private Input_PlayerMovement mInput;

    [SerializeField] private float mRotationAmount;
    [SerializeField] private float mRotationSpeed;
    private float mTargetRotation = 0f;

    void Awake()
    {
        mInput = new Input_PlayerMovement();

        mTargetRotation = transform.parent.rotation.eulerAngles.y;
    }
    void OnEnable() => mInput.Enable();
    void OnDisable() => mInput.Disable();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, Quaternion.Euler(0, mTargetRotation, 0), mRotationSpeed * Time.deltaTime);

        if (mInput.Camera.Rotation.triggered)
        {
            float axis = mInput.Camera.Rotation.ReadValue<float>();
            mTargetRotation += axis * mRotationAmount;
        }
    }
}
