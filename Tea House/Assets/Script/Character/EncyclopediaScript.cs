using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EncyclopediaScript : MonoBehaviour
{
    public List<GameObject> slotsPanels;
    int index = 0;

    public GameObject rightButton;
    public GameObject lefttButton;

    public GameObject infoPage;

    public Sprite lockedItem;
    private void Start()
    {
        CheckSlotsPanel();
    }
    private void Update()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Debug.Log("Update");
            UpdateEncyclopedia();
        }
    }
    void CheckSlotsPanel()
    {
        //Buttons
        if(index == 0)
        {
            lefttButton.SetActive(false);
            rightButton.SetActive(true);
        }
        else if(index == slotsPanels.Count - 1)
        {
            rightButton.SetActive(false);
            lefttButton.SetActive(true);
        }
        else
        {
            rightButton.SetActive(true);
            lefttButton.SetActive(true);
        }

        //Slots
        for (int i = 0; i < slotsPanels.Count; i++)
        {
            if (i == index)
            {
                slotsPanels[i].SetActive(true);
            }
            else
            {
                slotsPanels[i].SetActive(false);
            }
        }
    }

    public void UpdateEncyclopedia()
    {

        for (int i = 0; i < slotsPanels.Count; i++)
        {
            for(int j = 0; j < slotsPanels[i].transform.childCount; j++)
            {
                GameObject child = slotsPanels[i].transform.GetChild(j).gameObject;
                SlotScript slot = child.GetComponent<SlotScript>();

                slot.UpdateSlot(lockedItem);
            }
        }
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
    public void IncrementPanel()
    {
        index++;
        CheckSlotsPanel();
    }
    public void DerementPanel()
    {
        index--;
        CheckSlotsPanel();
    }
    public void CloseInfoPage()
    {
        infoPage.SetActive(false);
    }

}
