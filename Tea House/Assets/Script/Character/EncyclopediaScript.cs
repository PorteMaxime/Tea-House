using System.Collections.Generic;
using UnityEngine;

public class EncyclopediaScript : MonoBehaviour
{
    List<GameObject> slotsPanels;
    int index = 0;

    private void Start()
    {
        CheckSlotsPanel();
    }
    void CheckSlotsPanel()
    {
        if(index == 0)
        {
            //disable leftButtonSwap
        }
        else if(index == slotsPanels.Count - 1)
        {
            //disable rightButton
        }
        for (int i = 0; i < slotsPanels.Count; i++)
        {
            if(i != index)
                slotsPanels[i].SetActive(false);
            else slotsPanels[i].SetActive(true);
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

}
