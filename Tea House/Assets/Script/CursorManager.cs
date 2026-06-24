using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D[] cursorTextureAnim1;
    public Texture2D[] cursorTextureAnim2;
    public float frameRate;
    public int animIndex = 1;

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
