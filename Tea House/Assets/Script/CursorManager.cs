using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;

    private void Awake()
    {
        instance = this;
    }
    /*---*/
    public Image cursorImage;
    public GameObject cursorPos;
    //
    public Sprite defaultCursor;
    public Sprite[] cursorTextureAnim0;
    public Sprite[] cursorTextureAnim1;
    public Sprite[] cursorTextureAnim2;
    public float frameRate;
    public int animIndex = 0;
    public int lastAnimIndex = 0;

    private int frameCount;
    private int currentFrame;
    private float frameTimer;

    void Start()
    {
        cursorImage.sprite = defaultCursor;
        frameCount = cursorTextureAnim1.Length;
        Cursor.visible = false;
    }

    void Update()
    {
        if(lastAnimIndex != animIndex)
        {
            lastAnimIndex = animIndex;
            frameTimer = 0;
        }
        switch (animIndex)
        {
            case 0:
                cursorImage.sprite = defaultCursor;
                cursorPos.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                //pos -20/20
                break;
            case 1:
                DoAnim(cursorTextureAnim1);
                cursorPos.transform.localScale = new Vector3(1f, 1f, 1f);
                //pos -30/30
                break;
            case 2:
                DoAnim(cursorTextureAnim2);
                cursorPos.transform.localScale = new Vector3(1f, 1f, 1f);
                //pos -30/30
                break;
        }

        cursorPos.transform.position = Mouse.current.position.ReadValue();
    }
    void DoAnim(Sprite[] cursorAnim)
    {
        frameTimer -= Time.deltaTime;
        if (frameTimer < 0)
        {
            frameTimer += frameRate;
            currentFrame = (currentFrame + 1) % frameCount;
            cursorImage.sprite = cursorAnim[currentFrame];
        }
    }
}
