using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComputerScript : MonoBehaviour
{
    public Dictionary<string, GameObject> mCanvas = new Dictionary<string, GameObject>();
    public GameObject mActiveCanvas;

    [SerializeField] private InteractiveAreaScript interactionScript;
    private bool isComputerOn = false;

    private CameraController mCameraController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mCameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

        foreach (Transform child in gameObject.transform)
        {
            mCanvas.Add(child.name, child.gameObject);
            //child.gameObject.SetActive(true); // Uniquement pour les awake de chaque page
            child.gameObject.SetActive(false);

            if (mCanvas.Count() < 2)
                mActiveCanvas = child.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (interactionScript.isUsing != isComputerOn)
        {
            isComputerOn = !isComputerOn;

            if (interactionScript.isUsing == true)
                StartComputer();
            else
                ExitComputer();
        }
    }

    public void ChangeInterface(string interfaceName)
    {
        if (!mCanvas.ContainsKey(interfaceName))
            return;

        mActiveCanvas.SetActive(false);
        mActiveCanvas = mCanvas[interfaceName];
        mActiveCanvas.SetActive(true);
    }

    public void StartComputer()
    {
        ChangeInterface("CanvasHome");
    }

    public void ExitComputer()
    {
        mActiveCanvas.SetActive(false);

        interactionScript.Quit();
    }
}
