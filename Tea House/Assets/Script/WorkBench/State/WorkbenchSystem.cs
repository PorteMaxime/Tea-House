using UnityEngine;

public class WorkbenchSystem : MonoBehaviour
{
    public CameraController mCameraController;
    public Packaging_MainScript mPackaging;
    public Inventory inventory;

    public GameObject mBowl;
    public GameObject mMortar;
    public GameObject mMixtureBowl;
    public IngredientData mCurrentItem = null;
    public GameObject currentItemG = null;
    public IngredientData mMixture;
    public IngredientData mBruiseIngrdient;

    public WorkbenchStateMachine stateMachine;

    public SelectItemState selectItemState;
    public BruisingState bruisingState;

    void Awake()
    {
        mBruiseIngrdient = ScriptableObject.CreateInstance<IngredientData>();
        mMixture = ScriptableObject.CreateInstance<IngredientData>();
    }
    void Start()
    {
        stateMachine = new WorkbenchStateMachine();

        selectItemState = new SelectItemState(this);
        bruisingState = new BruisingState(this);

        stateMachine.ChangeState(selectItemState);
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void AddItemToMixture(IngredientData item)
    {
        mMixture.mEnergy += item.mEnergy;
        mMixture.mFreshness += item.mFreshness;
        mMixture.mWarmth += item.mWarmth;
        mMixture.mRelaxation += item.mRelaxation;
        mMixture.mPrestige += item.mPrestige;
    }

    public GameObject InstantiateGameObject(GameObject _gameObject, Vector3 pos)
    {
        return Instantiate(_gameObject, pos, Quaternion.identity);
    }
    public GameObject InstantiateGameObject(GameObject _gameObject, Vector3 pos, GameObject _parent)
    {
        return Instantiate(_gameObject, pos, Quaternion.identity, _parent.transform);
    }
    public void DestroyCurrentIngredient()
    {
        mCurrentItem = null;
        Destroy(currentItemG);
    }
}
