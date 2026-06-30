using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LabelTextureGenerator : AbstractDynamicTextureGenerator
{
    Image mPicture = null;
    TextMeshProUGUI mNumber = null;
    TextMeshProUGUI mBase = null;
    TextMeshProUGUI[] mIngredients = new TextMeshProUGUI[3];

    public override void Init(Camera camera)
    {
        base.Init(camera);

        // Get canva's fields to complete with the arguments of the function Generate
        mPicture = transform.Find("ProfilePicture").GetComponent<Image>();
        mNumber = transform.Find("OrderNumber").GetComponent<TextMeshProUGUI>();

        mBase = transform.Find("Base").GetComponent<TextMeshProUGUI>();
        mIngredients[0] = transform.Find("Ingredient1").GetComponent<TextMeshProUGUI>();
        mIngredients[1] = transform.Find("Ingredient2").GetComponent<TextMeshProUGUI>();
        mIngredients[2] = transform.Find("Ingredient3").GetComponent<TextMeshProUGUI>();

    }

    public override RenderTexture Generate(params object[] args)
    {
        // Is there enough parameters ? (6)
        if(args.Length <= 5 || args.Length > 6) return null;

        // Are they of the right type ?
        if (args[0] is Sprite == false || args[1] is string == false ||
            args[2] is BaseData == false ||
            (args[3] is IngredientData == false && args[3] != null) || (args[4] is IngredientData == false && args[4] != null) || (args[5] is IngredientData == false && args[5] != null)
            ) return null;

        // Cast arguments in the right type
        Sprite mailPicture = (Sprite)args[0];
        string mailNumber = (string)args[1];
        BaseData baseData = (BaseData)args[2];
        IngredientData ingredient1 = (IngredientData)args[3];
        IngredientData ingredient2 = (IngredientData)args[4];
        IngredientData ingredient3 = (IngredientData)args[5];

        // Init and Generate the texture
        return MakeTexture(
            mailPicture,
            mailNumber,
            baseData,
            ingredient1,
            ingredient2,
            ingredient3
        ); ;
    }

    private RenderTexture MakeTexture(Sprite mailPicture, string mailNumber,
        BaseData baseData,
        IngredientData ingredientData1, IngredientData ingredientData2, IngredientData ingredientData3)
    {
        // Texture Init
        mPicture.sprite = mailPicture;
        mNumber.text = mailNumber;
        mBase.text = baseData.mName;
        mIngredients[0].text = ingredientData1 != null ? ingredientData1.mName : "";
        mIngredients[1].text = ingredientData2 != null ? ingredientData2.mName : "";
        mIngredients[2].text = ingredientData3 != null ? ingredientData3.mName : "";

        // Texture Generation
        RenderTexture render = new RenderTexture(850, 1024, 16);
        mCamera.targetTexture = render;

        mCamera.enabled = true;
        gameObject.SetActive(true);

        mCamera.Render();

        gameObject.SetActive(false);
        mCamera.enabled = false;

        mTextures.Add(render);

        return render;
    }
}
