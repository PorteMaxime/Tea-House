using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    private Input_PlayerMovement mInput;

    private Rigidbody mRigidbody;

    [SerializeField] private GameObject mCameraOrigin;
    [SerializeField] private float mSpeed = 15f;

    void Awake() => mInput = new Input_PlayerMovement();
    void OnEnable() => mInput.Enable();
    void OnDisable() => mInput.Disable();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (mInput != null)
        {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        if(mInput.Character.Move.IsPressed())
        {
            Vector2 dir2 = mInput.Character.Move.ReadValue<Vector2>();
            Vector3 camDir = mCameraOrigin.transform.forward;

            Vector3 moveDirection = (mCameraOrigin.transform.forward * dir2.y + mCameraOrigin.transform.right * dir2.x).normalized;

            mRigidbody.linearVelocity = moveDirection * mSpeed;
        }
        else
            mRigidbody.linearVelocity = Vector3.zero;
    }
}
