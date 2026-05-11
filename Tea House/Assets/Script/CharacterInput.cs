using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    private PlayerMovement _input;

    [SerializeField] private float mSpeed = 15f;

    void Awake() => _input = new PlayerMovement();
    void OnEnable() => _input.Enable();
    void OnDisable() => _input.Disable();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if(_input.Character.Move.IsPressed())
        {
            Vector2 dir2 = _input.Character.Move.ReadValue<Vector2>();
            Vector3 dir3 = new Vector3(dir2.x, 0f, dir2.y);
            transform.Translate(dir3 * mSpeed * Time.deltaTime);
        }
    }
}
