using System.Collections;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    private Input_PlayerMovement mInput;

    private Rigidbody mRigidbody;

    [SerializeField] private GameObject mCameraOrigin;
    [SerializeField] private float mSpeed = 15f;
    private float rotationSpeed = 360f;
    public bool mCanMove = true;

    //Animator
    public Animator playerAnimator;
    float idleTimer = 0f;
    public float idleExtraDelay = 5f;

    public float sitExtraDelay = 6f;

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
        bool isMoving = mInput.Character.Move.IsPressed() && mCanMove;

        playerAnimator.SetBool("IsWalking", isMoving);

        if (isMoving)
        {
            idleTimer = 0f;

            Vector2 dir2 = mInput.Character.Move.ReadValue<Vector2>();
            Vector3 camDir = mCameraOrigin.transform.forward;

            Vector3 moveDirection = (mCameraOrigin.transform.forward * dir2.y + mCameraOrigin.transform.right * dir2.x).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            mRigidbody.linearVelocity = moveDirection * mSpeed;
        }
        else
        {
            mRigidbody.linearVelocity = Vector3.zero;

            idleTimer += Time.deltaTime;

            if (idleTimer >= idleExtraDelay)
            {
                StartCoroutine(ExtraIdle());
                idleTimer = 0f;
            }
        }
    }
    IEnumerator ExtraIdle()
    {
        playerAnimator.SetTrigger("IdleExtraTrigger");
        yield return new WaitForSeconds(4f);

        idleTimer = 0f;
    }
}
