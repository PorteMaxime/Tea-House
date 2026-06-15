using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PackagingScript : MonoBehaviour
{
    #region StateMachine
    public PackagingStateMachine mStateMachine;

    public PacketChoosingState mChoosing = null;
    public PacketOpening mOpening = null;
    public PacketCompleting mCompleting = null;
    public PacketFilling mFilling = null;
    public PacketLabelling mLabelling = null;
    public PacketLabelPrinting mPrinting = null;
    #endregion

    public GameObject mDebug = null;
    public IngredientData mDebugData = null;

    private InteractiveAreaScript mIArea = null;

    public CameraController mCameraController = null;
    public WorkingPoint mWorkingPoint = null;

    public PackageScript mOutput = null;
    public GameObject mOutputGO = null;
    void Start()
    {
        mCameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        mWorkingPoint = transform.Find("WorkingPoint").GetComponent<WorkingPoint>();
        mIArea = transform.Find("InteractiveArea").GetComponent<InteractiveAreaScript>();
        
        mStateMachine = new PackagingStateMachine();
        
        mChoosing = new PacketChoosingState(this);
        mOpening = new PacketOpening(this);
        mCompleting = new PacketCompleting(this);
        mFilling = new PacketFilling(this);
        mLabelling = new PacketLabelling(this);
        mPrinting = new PacketLabelPrinting(this);

        // TODO : Remove Debug
        mPrinting.mDebug = mDebug;
        mPrinting.mDebugData = mDebugData;

        mStateMachine.SwitchState(mChoosing);
    }
    void Update()
    {
        if(mIArea.isUsing == true)
            mStateMachine.Update();
    }

    public GameObject Instantiate(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        return GameObject.Instantiate(prefab, pos, rot);
    }
    public void Destroy(GameObject toDestroy)
    {
        GameObject.Destroy(toDestroy);
    }

    public void ShareRender(GameObject origin, GameObject target)
    {
        MeshFilter mesh = target.AddComponent<MeshFilter>();
        mesh.sharedMesh = origin.GetComponent<MeshFilter>().mesh;

        MeshRenderer meshRenderer = target.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterials = origin.GetComponent<MeshRenderer>().materials;
    }

    public void SetIngredientData(IngredientData data)
    {
        // TODO : Take the ingredient data from Workbench
    }
}
