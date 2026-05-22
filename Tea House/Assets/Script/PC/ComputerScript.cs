using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerScript : MonoBehaviour
{
    public List<GameObject> mInterfaces = new List<GameObject>();
    public GameObject mActiveInterface;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Nombre de pages : " + gameObject.transform.childCount);

        int i = 0;

        foreach (Transform child in gameObject.transform)
        {
            mInterfaces.Add(child.gameObject);
            child.gameObject.SetActive(false);

            //Button test = new Button();

            //test.parent = gameObject;
        }

        if (mInterfaces.Count > 0)
        {
            mActiveInterface = mInterfaces[0];
            mActiveInterface.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayInterface(int interfaceNumber)
    {
        if (interfaceNumber >= 0 && mInterfaces.Count > interfaceNumber)
        {
            mActiveInterface.SetActive(false);
            mActiveInterface = mInterfaces[interfaceNumber];
            mActiveInterface.SetActive(true);
        }
    }
}
