using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableScript : MonoBehaviour
{
    bool isUsing = false;
    public InteractiveAreaScript mInteractiveArea;
    public GameObject mTablePanel;
    private GameObject mMainPanel;

    public List<GameObject> mPackets;
    private void Start()
    {
        mMainPanel = GameObject.FindGameObjectWithTag("MainPanel");
    }

    void Update()
    {
        if (mInteractiveArea.isUsing != isUsing)
        {
            isUsing = mInteractiveArea.isUsing;

            if (isUsing) {
                mMainPanel.SetActive(false);
                StartCoroutine(ShowPanel());
            }
            else
            {
                mMainPanel.SetActive(true);
                mTablePanel.SetActive(false);
            }
        }
    }
    IEnumerator ShowPanel()
    {
        yield return new WaitForSeconds(0.5f);
        mTablePanel.SetActive(true);
    }
}


