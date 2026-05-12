using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    private Input_PlayerMovement mInput;
    [SerializeField] private GameObject mCameraOrigin;
    [SerializeField] private float mSpeed = 15f;

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
        if(mInput != null)
        {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        if(mInput.Character.Move.IsPressed())
        {
            Vector2 dir2 = mInput.Character.Move.ReadValue<Vector2>();
            Vector3 dir3 = new Vector3(dir2.x, 0f, dir2.y);

            Quaternion tmpRot = transform.rotation;
            transform.rotation = mCameraOrigin.transform.rotation;
            transform.Translate(dir3 * mSpeed * Time.deltaTime);
            transform.rotation = tmpRot;
        }
    }
}
