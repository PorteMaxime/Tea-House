using UnityEngine;
using UnityEngine.UI;

public class StaticCornerRadius : MonoBehaviour
{
    [SerializeField] private int mCornerRadius = 0;

    void Awake()
    {
        Image image = GetComponent<Image>();
        RectTransform rectTransform = transform as RectTransform;

        if (image == null || rectTransform == null)
            return;

        Vector2 size = rectTransform.rect.size;
        int width = Mathf.Max((int)size.x, 4);
        int height = Mathf.Max((int)size.y, 4);
        int radius = Mathf.Min(mCornerRadius, Mathf.Min(width / 2, height / 2));

        // Création de la texture aux dimensions exactes du RectTransform
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[width * height];
        Color32 white = new Color32(0, 0, 0, 255);
        Color32 transparent = new Color32(0, 0, 0, 0);

        // Algorithme de dessin de l'arrondi
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (IsInsideRoundedRect(x, y, width, height, radius))
                    pixels[y * width + x] = white;
                else
                    pixels[y * width + x] = transparent;
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        // Application du sprite final
        Sprite generatedSprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        image.sprite = generatedSprite;
        image.type = Image.Type.Simple;
    }

    private bool IsInsideRoundedRect(int x, int y, int width, int height, int r)
    {
        if (x < r && y < r) return Vector2.Distance(new Vector2(x, y), new Vector2(r, r)) <= r;
        if (x > width - r - 1 && y < r) return Vector2.Distance(new Vector2(x, y), new Vector2(width - r - 1, r)) <= r;
        if (x < r && y > height - r - 1) return Vector2.Distance(new Vector2(x, y), new Vector2(r, height - r - 1)) <= r;
        if (x > width - r - 1 && y > height - r - 1) return Vector2.Distance(new Vector2(x, y), new Vector2(width - r - 1, height - r - 1)) <= r;

        return true;
    }
}