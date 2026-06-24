using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;

    private void Awake()
    {
        instance = this;
    }

    public Texture2D defaultCursor;
    public Texture2D[] cursorTextureAnim0;
    public Texture2D[] cursorTextureAnim1;
    public Texture2D[] cursorTextureAnim2;
    public float frameRate;
    public int animIndex = 0;
    public int lastAnimIndex = 0;

    private int frameCount;
    private int currentFrame;
    private float frameTimer;

    void Start()
    {
        Cursor.SetCursor(cursorTextureAnim1[0], Vector2.zero, CursorMode.Auto);
        frameCount = cursorTextureAnim1.Length;
    }

    void Update()
    {
        if (InputManager.Get().mPlayerInteract.Player.Select.IsPressed() == true)
        {
            animIndex = 2;
        }
        if(lastAnimIndex != animIndex)
        {
            lastAnimIndex = animIndex;
            frameTimer = 0;
        }
        switch (animIndex)
        {
            case 0:
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
            case 1:
                DoAnim(cursorTextureAnim1);
                break;
            case 2:
                DoAnim(cursorTextureAnim2);
                break;
        }
    }
    void DoAnim(Texture2D[] cursorAnim)
    {
        frameTimer -= Time.deltaTime;
        if (frameTimer < 0)
        {
            frameTimer += frameRate;
            currentFrame = (currentFrame + 1) % frameCount;
            Cursor.SetCursor(cursorAnim[currentFrame], Vector2.zero, CursorMode.Auto);
        }
    }
}
