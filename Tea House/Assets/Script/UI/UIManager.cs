using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public MainPanel mMainPanel;
    public GameObject mWorckBenchPanel;
    public TablePanel mTablePanel;


    private void Start()
    {
        mMainPanel.gameObject.SetActive(true);
    }
    void Update()
    {
        mMainPanel.OnUpdateUI();
        //mTablePanel.OnUpdateUI();
    }
}
