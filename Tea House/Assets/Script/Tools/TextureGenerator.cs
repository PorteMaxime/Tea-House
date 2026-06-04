using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class AbstractDynamicTextureGenerator : MonoBehaviour
{
    protected Camera mCamera = null;
    protected List<RenderTexture> mTextures = new List<RenderTexture>();

    public virtual void Init(Camera camera)
    {
        mCamera = camera;
        // Get canva's fields to complete with the arguments of the function Generate
    }

    void OnDestroy()
    {
        foreach (RenderTexture texture in mTextures)
        {
            texture.Release();
            RenderTexture.Destroy(texture);
        }
    }

    // heriage to generate your own texture
    public abstract RenderTexture Generate(params object[] args);
}

public class DynamicTextureManager
{
    #region Singleton
    static DynamicTextureManager sInstance = null;
    public static DynamicTextureManager Get()
    {
        if (sInstance == null)
            sInstance = new DynamicTextureManager();
        return sInstance;
    }

    #endregion
    
    private Camera mCamera = null;

    private Dictionary<DynamicTexture, AbstractDynamicTextureGenerator> mGenerators = new Dictionary<DynamicTexture, AbstractDynamicTextureGenerator>();

    public enum DynamicTexture
    {
        None,

        Label,

        Count
    };

    private DynamicTextureManager()
    {
        mCamera = GameObject.Find("Camera_DynamicTexture").GetComponent<Camera>();
    }

    public static bool AddGenerator<GeneratorType>(DynamicTexture id, GameObject generatorPrefabInstance) where GeneratorType : AbstractDynamicTextureGenerator
    {
        if (generatorPrefabInstance == null || generatorPrefabInstance.GetComponent<Canvas>() == null ||
            id <= DynamicTexture.None || id >= DynamicTexture.Count) return false;


        DynamicTextureManager manager = DynamicTextureManager.Get();
        if (manager.mGenerators.ContainsKey(id)) return false;

        GeneratorType generator = generatorPrefabInstance.AddComponent<GeneratorType>();
        generator.Init(manager.mCamera);
        manager.mGenerators.Add(id, generator);

        return true;
    }

    public static void RemoveGenerator(DynamicTexture id)
    {
        if (id <= DynamicTexture.None || id >= DynamicTexture.Count) return;

        DynamicTextureManager manager = sInstance;
        if(manager.mGenerators.ContainsKey(id) == false) return;

        Component.Destroy(manager.mGenerators[id].gameObject.GetComponent<AbstractDynamicTextureGenerator>());
        manager.mGenerators.Remove(id);
    }

    public static AbstractDynamicTextureGenerator GetGenerator(DynamicTexture id)
    {
        if (id <= DynamicTexture.None || id >= DynamicTexture.Count) return null;

        DynamicTextureManager manager = DynamicTextureManager.Get();
        if (manager.mGenerators.ContainsKey(id) == false) return null;

        return manager.mGenerators[id];
    }

    public static RenderTexture Generate(DynamicTexture id, params object[] parameters)
    {
        if (id <= DynamicTexture.None || id >= DynamicTexture.Count) return null;

        DynamicTextureManager manager = DynamicTextureManager.Get();
        if (manager.mGenerators.ContainsKey(id) == false) return null;

        if (manager.mGenerators[id] == null)
        {
            manager.mGenerators.Remove(id);
            return null;
        }
        return manager.mGenerators[id].Generate(parameters);
    }
}